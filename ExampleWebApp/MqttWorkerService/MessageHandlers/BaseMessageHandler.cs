using System.Text.Json;
using System.Text.Json.Serialization;
using Database.Entities;
using Database.Repositories;
using DataModels.ApiModels;
using DataModels.Models;

namespace MqttWorkerService;

public class BaseMessageHandler<T>(ILogger<BaseMessageHandler<T>> logger, IEventRepository eventRepository) : IMessageHandler<T, EventBaseDbEntity> 
    where T : BaseMessage
{
    public virtual Task<bool> Handle(T @event)
    {
        return Task.FromResult(true);
    }

    public virtual async Task<HandleEventResponse> Handle(EventBaseDbEntity @event)
    {
        return new HandleEventResponse
        {
            Success = false,
            FaultReason = "not implemented",
            TargetId = default
        };
    }

    public async Task<bool> Execute(EventBaseDbEntity @event)
    {
        var success = await Handle(@event);
        return await ProcessEvent(@event, success, !success.HackParameter);
    }

    public async Task<bool> ProcessEvent(EventBaseDbEntity @event, HandleEventResponse handleResult, bool createProcessed = false) 
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));
        await eventRepository.ProvessEvent(@event, handleResult, createProcessed);
        return handleResult.Success;
    }

    public T Extract(EventBaseDbEntity entity)
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