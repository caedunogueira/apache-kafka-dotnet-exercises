using ClientGateway;
using ClientGateway.Controllers;
using ClientGateway.Domain;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<SchemaRegistryConfig>(builder.Configuration.GetSection("SchemaRegistry"));

builder.Services.AddSingleton<IProducer<string, Biometrics>>(sp =>
{
    var config = sp.GetRequiredService<IOptions<ProducerConfig>>();
    var schemaRegistryClient = sp.GetRequiredService<ISchemaRegistryClient>();

    return new ProducerBuilder<string, Biometrics>(config.Value)
        .SetValueSerializer(new JsonSerializer<Biometrics>(schemaRegistryClient))
        .Build();
});

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();

    return new CachedSchemaRegistryClient(config.Value);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();