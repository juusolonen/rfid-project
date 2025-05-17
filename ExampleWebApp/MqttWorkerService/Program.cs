using System.Diagnostics;
using System.Text.Json;
using Database;
using Database.Entities;
using Database.Repositories;
using MQTTnet;
using MQTTnet.Extensions.TopicTemplate;

namespace MqttWorkerService;

public class Program
{
    private static Process mosquittoProcess;
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<HostOptions>(o => o.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);
        
        builder.AddDb();
        builder.AddRepositories();
        // Start Mosquitto process
        //builder.StartMosquitto(mosquittoProcess);

        // Handle process exit event
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        
        builder.AddServices();
        
        var host = builder.Build();
        await SubscribeToMosquitto(host.Services);

        host.CheckMigrations();
        
        host.Run();
    }
    
     public static async Task SubscribeToMosquitto(IServiceProvider services)
    {
        var mqttFactory = new MqttClientFactory();
        IMqttClient mqttclient = mqttFactory.CreateMqttClient();
        MqttClientOptions options = new MqttClientOptionsBuilder()
            .WithClientId("MqttWorkerService")
            .WithTcpServer("host.docker.internal", 1883)
            .WithCredentials("", "")
            .Build();

        string deviceName = Environment.GetEnvironmentVariable("GREENGRASS_DEVICE_NAME");
        string topic = $"{deviceName}/{MqttWorkerServiceConstants.GreengrassDevicePublishTopic}";
        MqttTopicTemplate sampleTemplate = new(topic);

        mqttclient.ApplicationMessageReceivedAsync += async e =>
        {

            var jsonString = e.ApplicationMessage.ConvertPayloadToString();
            
            using (var document = JsonDocument.Parse(jsonString))
            {
                using (var scope = services.CreateScope())
                {
                    var newEvent = new EventBaseDbEntity
                    {
                        Processed = false,
                        ReceivedAt = DateTime.UtcNow,
                        Data = document
                    };

                    var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                    await repository.Add(newEvent);
                }
 
            }
            
            Console.WriteLine($"Received message: {e.ApplicationMessage.ConvertPayloadToString()}");
        };

        var connection = await mqttclient.ConnectAsync(options, CancellationToken.None);

        if (connection.ResultCode == MqttClientConnectResultCode.Success)
        {
            Console.WriteLine("Connected to MQTT broker successfully.");

            // Subscribe to a topic
            var mqttFactory2 = new MqttClientFactory();
            var mqttSubscribeOptions =
                mqttFactory2.CreateSubscribeOptionsBuilder().WithTopicTemplate(sampleTemplate).Build();

            await mqttclient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }
        else
        {
            Console.WriteLine($"Failed to connect to MQTT broker: {connection.ResultCode}");
        }
    }

    private static void OnProcessExit(object sender, EventArgs e)
    {
        try
        {
            // Kill the Mosquitto process
            if (mosquittoProcess != null && !mosquittoProcess.HasExited)
            {
                mosquittoProcess.Kill();
                Console.WriteLine("Mosquitto process killed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error killing Mosquitto: {ex.Message}");
        }
    }
}