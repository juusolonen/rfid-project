using System.Text.Json;
using Database.Entities;
using Database.Repositories;
using DataModels.ApiModels;
using DataModels.Models;

namespace MqttWorkerService;

public abstract class TagMessageHandler<T, TEntity>(ILogger<TagMessageHandler<T, TEntity>> logger, IEventRepository eventRepository) : IMessageHandler<T, TEntity> 
    where T : TagMessageBase 
    where TEntity : EventBaseDbEntity
{
    public abstract Task<bool> Handle(T @event);
    public abstract Task<HandleEventResponse> Handle(TEntity @event);
    

    public async Task<bool> Execute(TEntity @event)
    {
        var result = await Handle(@event);
        return await ProcessEvent(@event, result);
    }

    public async Task<bool> ProcessEvent(TEntity @event, HandleEventResponse handleResult, bool createProcessed = false) 
    {
        ArgumentNullException.ThrowIfNull(@event);
        await eventRepository.ProvessEvent(@event, handleResult);
        return handleResult.Success;
    }
    
    public T Extract(TEntity entity)
    {
        var options = JsonSerializerSettings_.GetDefaults();

        var root = entity.Data.RootElement;
        
        T message;
        try
        {
            message = (T)root.Deserialize(typeof(T), options)!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return default;
        }

        return message;
    }
}