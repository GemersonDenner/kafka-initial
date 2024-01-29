using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using Producer;

Console.WriteLine("Starting Consumer Json");


var conf = new ConsumerConfig
{
    GroupId = "ConsumerJson",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

JsonSerializerConfig jsonSerializerConfig = new JsonSerializerConfig{BufferBytes=100};

using(var c = new ConsumerBuilder<string, Message>(conf)
.SetKeyDeserializer(Deserializers.Utf8)
.SetValueDeserializer(new JsonDeserializer<Message>().AsSyncOverAsync())
.SetErrorHandler((_,e)=> Console.WriteLine($"Error: {e.Reason}"))
.Build()
)
{
    c.Subscribe("MessageTopicJson");
    CancellationTokenSource cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>{
        e.Cancel = true;
        cts.Cancel();
    };
    
    try
    {
        while(true)
        {
            try
            {
                var messageConsumed = c.Consume(cts.Token);
                var messageJson = JsonSerializer.Serialize(messageConsumed.Message.Value);
                Console.WriteLine($"Message: {messageJson}");
            }
            catch(ConsumeException ex)
            {
                Console.WriteLine($"Error occurred: {ex.Error.Reason}");
            }
        }
    }
    catch(OperationCanceledException){ c.Close();}
}

