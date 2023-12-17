using Confluent.Kafka;
using HeartRateZoneService.Domain;
using static Confluent.Kafka.ConfigPropertyNames;

namespace HeartRateZoneService.Workers;

public class HeartRateZoneWorker : BackgroundService
{
    private const string BiometricsImportedTopicName = "BiometricsImported";
    private const string HeartRateZoneReachedTopicName = "HeartRateZoneReached";
    
    private readonly IConsumer<string, Biometrics> _consumer;
    private readonly IProducer<string, HeartRateZoneReached> _producer;
    private readonly ILogger<HeartRateZoneWorker> _logger;

    private TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    public HeartRateZoneWorker(IConsumer<string, Biometrics> consumer,
        IProducer<string, HeartRateZoneReached> producer,
        ILogger<HeartRateZoneWorker> logger)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        logger.LogInformation("HeartRateZoneWorker is Active.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _producer.InitTransactions(DefaultTimeout);
        _consumer.Subscribe(BiometricsImportedTopicName);

        while (!stoppingToken.IsCancellationRequested) 
        {
            var consumeResult = _consumer.Consume(stoppingToken);

            await HandleMessage(consumeResult.Message.Value, stoppingToken);
        }

        _consumer.Close();

        await Task.Delay(1000);
    }   

    protected virtual async Task HandleMessage(Biometrics biometrics, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Message Received: {biometrics.DeviceId}");

        var topicPartitionOffsets = _consumer.Assignment.Select(topicPartition =>
            new TopicPartitionOffset(topicPartition, _consumer.Position(topicPartition))
        );

        _producer.BeginTransaction();
        _producer.SendOffsetsToTransaction(topicPartitionOffsets, _consumer.ConsumerGroupMetadata, DefaultTimeout);

        try
        {
            await Task.WhenAll(biometrics.HeartRates
                .Where(hr => hr.GetHeartRateZone(biometrics.MaxHeartRate) != HeartRateZone.None)
                .Select(hr =>
                {
                    var zone = hr.GetHeartRateZone(biometrics.MaxHeartRate);
                    var heartRateZoneReached = new HeartRateZoneReached(
                        biometrics.DeviceId,
                        zone,
                        hr.DateTime,
                        hr.Value,
                        biometrics.MaxHeartRate);
                    var message = new Message<string, HeartRateZoneReached>
                    {
                        Key = biometrics.DeviceId.ToString(),
                        Value = heartRateZoneReached
                    };

                    return _producer.ProduceAsync(HeartRateZoneReachedTopicName, message, cancellationToken);
                }));

            _producer.CommitTransaction();
        }
        catch (Exception ex)
        {
            _producer.AbortTransaction();
            throw new Exception("Transaction Failed", ex);
        }
    }
}
