using System.Text.Json;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{

    private readonly ILogger<MessageController> _logger;
    const string schemaRegistryAddress = "http://localhost:8081";
    const string brokerAddress = "localhost:9092";
    const string topicName = "MessageTopic";

    public MessageController(ILogger<MessageController> logger)
    {
        _logger = logger;
    }

    [HttpPost("PostJson")]
    public async Task<IResult> PostJson(string text)
    {
        var message = new Message(text);
        JsonSerializerConfig jsonSerializerConfig = new JsonSerializerConfig{BufferBytes=100};
        var schemaRegistryConfig = new SchemaRegistryConfig{Url=schemaRegistryAddress};
        var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        var config = new ProducerConfig{BootstrapServers=brokerAddress};

        using(var p = new ProducerBuilder<string, Message>(config)
        .SetValueSerializer(new JsonSerializer<Message>(schemaRegistry, jsonSerializerConfig))
        .Build()
        )
        {
            try
            {
                var produce = await p.ProduceAsync(topicName, new Message<string, Message>{Value = message});
                return Results.Created("/PostJson", message);
            }
            catch(ProduceException<Null, string> ex)
            {
                return Results.Problem(ex.Error.Reason);
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
            
        }
    }

    [HttpPost("PostAvro")]
    public IResult PostAvro(string text)
    {
        var message = new Message(text);
        return Results.Created("/PostAvro", message);
    }
}
