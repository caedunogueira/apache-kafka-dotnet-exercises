using System.Diagnostics.Metrics;
using System.Net;
using System.Text;
using ClientGateway.Domain;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ClientGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientGatewayController : ControllerBase
{
    private const string BiometricsImportedTopicName = "BiometricsImported";

    private readonly IProducer<string, Biometrics> _producer;
    private readonly ILogger<ClientGatewayController> _logger;
    public ClientGatewayController(IProducer<string, Biometrics> producer, 
        ILogger<ClientGatewayController> logger)
    {
        _producer = producer;
        _logger = logger;
        
        logger.LogInformation("ClientGatewayController is Active.");
    }

    [HttpGet("Hello")]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public String Hello()
    {
        _logger.LogInformation("Hello World");

        return "Hello World";
    }

    [HttpPost("Biometrics")]
    [ProducesResponseType(typeof(Biometrics), (int)HttpStatusCode.Accepted)]
    public async Task<AcceptedResult> RecordMeasurements(Biometrics metrics)
    {
        var message = new Message<string, Biometrics> 
        { 
            Key = metrics.DeviceId.ToString(),
            Value = metrics 
        };

        _logger.LogInformation("Accepted biometrics");

        _ = await _producer.ProduceAsync(BiometricsImportedTopicName, message);

        _producer.Flush();

        return Accepted("", metrics);
    }
}



