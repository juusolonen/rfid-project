using System.Diagnostics;
using System.Text.Json;
using Database;
using Database.Repositories;
using DataModels.ApiModels;
using MqttWorkerService.MessageHandlers;

namespace MqttWorkerService;

public class EventProcessingBackgroundService(IServiceProvider serviceProvider, ILogger<EventProcessingBackgroundService> logger)
    : BackgroundService
{

    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = new Stopwatch();
            sw.Start();
            (var Processed, var Total) = await ProcessEvents(stoppingToken);
            sw.Stop();
            logger.LogInformation("Processed {processEvents}/{Total} events in {time}", Processed, Total, sw.Elapsed);
            
            await Task.Delay(5000, stoppingToken);
    
        }
    }

    private async Task<(int,int)> ProcessEvents(CancellationToken stoppingToken)
    {
        int processed = 0;
        int total = 0;
        await _semaphoreSlim.WaitAsync(stoppingToken);
        try
        {
            using var scope = serviceProvider.CreateScope();
            var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var dispatcher = scope.ServiceProvider.GetRequiredService<EventModifierDispatcher>();
            var context = scope.ServiceProvider.GetRequiredService<RfidDatabaseContext>();
            var events = await eventRepository.GetAllUnprocessedEvents();

            total = events.Count;
            logger.LogInformation("Found {events} unprocessed events", total);
            foreach (var @event in events)
            {
                
                var success = await dispatcher.Dispatch(@event);

                if (success)
                {
                    processed++;
                }

                await context.SaveChangesAsync(stoppingToken);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return (processed, total);

    }
    
}