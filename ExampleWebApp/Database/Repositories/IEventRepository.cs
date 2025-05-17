using Database.Entities;
using DataModels.Models;

namespace Database.Repositories;

public interface IEventRepository
{
    public Task Add(EventBaseDbEntity entity);
    
    public Task<List<EventBaseDbEntity>> GetAllUnprocessedEvents(bool faulted = false);
    
    public Task<List<EventBaseDbEntity>> GetAllEvents();
    
    public Task<int> ProvessEvent(EventBaseDbEntity entity, HandleEventResponse eventResponse, bool createProcessed = false);

    public Task<(EventBaseDbEntity?, UserDbEntity?, UserDbEntity?, List<ToolDbEntity>)> GetEventAsync(Guid id);
}