using System.Diagnostics;
using Database;
using Database.Entities;
using Database.Repositories;
using DataModels.ApiModels;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MqttWorkerService.MessageHandlers;

namespace MqttWorkerService;

public static class BuilderExtensions
{
    public static void AddDb(this HostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(MqttWorkerServiceConstants.DatabaseName);

        builder.Services.AddDbContext<RfidDatabaseContext>(options =>
        {
            options.UseNpgsql(connectionString,
                o  => o.SetPostgresVersion(14, 17) 
                    );
        });
    }
    
    public static void AddRepositories(this HostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventRepository, EventRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IToolRepository, ToolRepository>();
    }
    
    public static void AddServices(this HostApplicationBuilder builder)
    {
        builder.AddMessageHandlers();

        builder.Services.AddScoped<EventModifierDispatcher>();
        
        var mqttFactory = new MqttClientFactory();
        IMqttClient mqttclient = mqttFactory.CreateMqttClient();
        
        builder.Services.AddSingleton<IMqttClient>(mqttclient);
        builder.Services.AddHostedService<EventProcessingBackgroundService>();
    }

    public static void AddMessageHandlers(this HostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMessageHandler<NewTag, EventBaseDbEntity>, NewHandler>();
        builder.Services.AddScoped<IMessageHandler<DeleteTag, EventBaseDbEntity>, DeleteHandler>();
        builder.Services.AddScoped<IMessageHandler<BorrowMessage, EventBaseDbEntity>, BorrowMessageHandler>();
        builder.Services.AddScoped<IMessageHandler<UserLogin, EventBaseDbEntity>, BaseMessageHandler<UserLogin>>();
        builder.Services.AddScoped<IMessageHandler<UserLogout, EventBaseDbEntity>, BaseMessageHandler<UserLogout>>();
        builder.Services.AddScoped<IMessageHandler<AdminLogout, EventBaseDbEntity>, BaseMessageHandler<AdminLogout>>();
        builder.Services.AddScoped<IMessageHandler<AdminRead, EventBaseDbEntity>, BaseMessageHandler<AdminRead>>();
        builder.Services.AddScoped<IMessageHandler<CreateAdmin, EventBaseDbEntity>, BaseMessageHandler<CreateAdmin>>();
        builder.Services.AddScoped<IMessageHandler<InitMessage, EventBaseDbEntity>, BaseMessageHandler<InitMessage>>();
   
    }
    
    public static void StartMosquitto(this HostApplicationBuilder builder, Process mosquittoProcess)
    {
        try
        {
            var enviromentPath = builder.Configuration.GetValue<string>("PATH");

            var paths = enviromentPath.Split(':');
            var exePath = paths.Where(path => path.Contains("mosquitto")).First();

            if (string.IsNullOrWhiteSpace(exePath))
            {
                Console.WriteLine("Mosquitto not found in PATH.");
                Process.GetCurrentProcess().Kill();
            }

            if (!exePath.EndsWith("/mosquitto"))
            {
                exePath += "/mosquitto";
            }
            
            // Start Mosquitto using Process.Start
            mosquittoProcess = new Process();
            mosquittoProcess.StartInfo.FileName = exePath; // Make sure mosquitto is in your PATH
            
            var mosquittoConf = builder.Configuration.GetValue<string>("MOSQUITTO_CONF_FILE");
            
            if (!string.IsNullOrWhiteSpace(mosquittoConf) && File.Exists(mosquittoConf))
            {
                mosquittoProcess.StartInfo.Arguments = $"-c {mosquittoConf}";
            }
            mosquittoProcess.Start();
            Console.WriteLine("Mosquitto started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting Mosquitto: {ex.Message}");
            Process.GetCurrentProcess().Kill();
        }
    }
    
    public static void CheckMigrations(this IHost host)
    {
        using var serviceScope = host.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<RfidDatabaseContext>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations();

        if (pendingMigrations.Any())
        {
            dbContext.Database.Migrate();
        }
    }
}