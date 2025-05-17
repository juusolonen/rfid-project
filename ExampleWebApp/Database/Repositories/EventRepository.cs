using Database.Entities;
using DataModels.ApiModels;
using DataModels.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class EventRepository(RfidDatabaseContext context) : IEventRepository
{
    public async Task Add(EventBaseDbEntity entity)
    {
        await context.Events.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<List<EventBaseDbEntity>> GetAllUnprocessedEvents(bool faulted = false)
    {
        return await context.Events
            .Include(e => e.Tools)
            .Where(e => !e.Processed && e.Faulted == faulted).ToListAsync();
    }

    public async Task<List<EventBaseDbEntity>> GetAllEvents()
    {
        return await context.Events
            .Include(e => e.Tools)
            .Include(e => e.Caller)
            .Include(e => e.TargetUser)
            .ToListAsync();
    }

    public async Task<int> ProvessEvent(EventBaseDbEntity entity, HandleEventResponse eventResponse, bool createProcessed = false)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            context.Attach(entity);
        }

        if (eventResponse.TagType == TagType.USER)
        {
            entity.TargetUserId = eventResponse.TargetId;
        }
        
        entity.CallerId = eventResponse.CallerId;

        if (eventResponse.Success)
        {
            entity.Processed = true;
            var processedEvent = ProcessedEventDbEntity.FromEventBaseDbEntity(entity);
            await context.AddAsync(processedEvent);
        }
        else
        {
            entity.Faulted = true;
            entity.FaultReason = eventResponse.FaultReason;
            if (createProcessed)
            {
                var processedEvent = ProcessedEventDbEntity.FromEventBaseDbEntity(entity);
                await context.AddAsync(processedEvent);
            }
            return 0;
        }

        return 1;
    }

    public async Task<(EventBaseDbEntity?, UserDbEntity?, UserDbEntity?, List<ToolDbEntity>)> GetEventAsync(Guid id)
    {
        var @event = await context.Events
            .Include(t => t.Caller)
            .Include(t => t.TargetUser)
            .Include(t => t.Tools)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        return (@event, @event?.TargetUser, @event?.Caller, @event?.Tools ?? []);
    }
}