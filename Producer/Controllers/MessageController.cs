using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;
using NamespageMessage;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{

    private readonly ILogger<MessageController> _logger;
    const string schemaRegistryAddress = "http://localhost:8081";
    const string brokerAddress = "localhost:9092";
    const string topicNameJson = "MessageTopicJson";
    const string topicNameAvro = "MessageTopicAvro";

    public MessageController(ILogger<MessageController> logger)
    {
        _logger = logger;
        this.RegisterAvro();
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
                var produce = await p.ProduceAsync(topicNameJson, new Message<string, Message>{Value = message});
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
    public async Task<IResult> PostAvro(string text)
    {
        var message = new AvroMessage{Id=Guid.NewGuid().ToString(), CreatedDate=DateTime.UtcNow.ToString(), Text=text};
        var schemaRegistryConfig = new SchemaRegistryConfig{Url=schemaRegistryAddress};
        var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        var config = new ProducerConfig{BootstrapServers=brokerAddress};

        using(var p= new ProducerBuilder<string, AvroMessage>(config)
        .SetValueSerializer(new AvroSerializer<AvroMessage>(schemaRegistry))
        .Build()
        )
        {
            try
            {
                var produce = await p.ProduceAsync(topicNameAvro, new Message<string, AvroMessage>{Value = message});
                return Results.Created("/PostAvro", message);
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

    private async void RegisterAvro()
    {
        var addressRegisterAvro = "http://localhost:8081/subjects/AvroMessage/versions";
        var avroContent = System.IO.File.ReadAllText("Model/avro-Message.avsc");
        var requestData = new { schema = avroContent, schemaType = "AVRO" };

        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = new HttpClient();
        var response = await client.PostAsync(addressRegisterAvro, content);
    }

}
