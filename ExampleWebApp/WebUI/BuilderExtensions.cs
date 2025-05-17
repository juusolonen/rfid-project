using Database;
using Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WebUI;

public static class BuilderExtensions
{
    public static void AddDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(MqttWorkerServiceConstants.DatabaseName);

        builder.Services.AddDbContext<RfidDatabaseContext>(options =>
        {
            options.UseNpgsql(connectionString,
                o  => o.SetPostgresVersion(14, 17) 
                    );
        });
    }
    
    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventRepository, EventRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IToolRepository, ToolRepository>();
    }
}