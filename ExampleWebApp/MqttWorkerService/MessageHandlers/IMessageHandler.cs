using Database.Entities;
using DataModels.ApiModels;
using DataModels.Models;

namespace MqttWorkerService;

public interface IMessageHandler<TEvent, TDBEntity>
    where TEvent : BaseMessage 
    where TDBEntity : EventBaseDbEntity
{ 
    Task<bool> Handle(TEvent @event);
    Task<HandleEventResponse> Handle(TDBEntity @event);
    
    public Task<bool> Execute(TDBEntity @event);
    
    public Task<bool> ProcessEvent(TDBEntity @event, HandleEventResponse handleResult, bool createProcessed = false);

    public TEvent Extract(TDBEntity entity);
}