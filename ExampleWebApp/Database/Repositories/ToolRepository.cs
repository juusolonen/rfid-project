using Database.Entities;
using DataModels.ApiModels;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class ToolRepository(RfidDatabaseContext context) : IToolRepository
{
    public async Task<ToolDbEntity?> GetActiveToolByTagEPCAsync(long id)
    {
        return await context.Tools
            .Include(tool => tool.TargetedEvents)
            .FirstOrDefaultAsync(u => u.TagIdentifier == id && !u.Deleted);
    }
    
    public async Task<ToolDbEntity?> GetToolByNameAndSlotAsync(string name, int slot)
    {
        return await context.Tools.FirstOrDefaultAsync(u => u.Name == name && u.Slot == slot);
    }

    public async Task<List<ToolDbEntity>> GetAllToolsAsync()
    {
        return await context.Tools.Include(t => t.TargetedEvents).ToListAsync();
    }

    public async Task<bool> UpdateToolBorrowInfo(BorrowMessage message, EventBaseDbEntity @event, Guid? callerId = null)
    {
        bool success = false;
        foreach (var toolInEvent in message.Tools)
        {
            var existingTool = await GetActiveToolByTagEPCAsync(toolInEvent.Id);

            if (existingTool != null)
            {
                success = true;
                
                var now = DateTime.UtcNow;
                existingTool.In = toolInEvent.In;
                existingTool.Out = toolInEvent.Out;
                existingTool.BorrowedAt = toolInEvent.Out ? now : null;
                existingTool.LastBorrower = toolInEvent.Out ? message.User : null;
                existingTool.ReturnedAt = toolInEvent.In ? now : null;
                existingTool.TargetedEvents.Add(@event);
            }
            else
            {
                var faultedEvent = ProcessedEventDbEntity.FaultedFromEvent(message, @event, toolInEvent.Name, callerId);
                await context.AddAsync(faultedEvent);
            }
        }
        
        await context.SaveChangesAsync();
        return success;
    }

    public async Task<(ToolDbEntity?, List<ProcessedEventDbEntity>)> GetToolAsync(Guid id)
    {
        var tool = await context.Tools
            .Include(t => t.TargetedEvents)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        return (tool, tool?.TargetedEvents.OfType<ProcessedEventDbEntity>().ToList() ?? new List<ProcessedEventDbEntity>());
    }

    public async Task<ToolDbEntity> CreateToolFromTag(Tag tag, EventBaseDbEntity @event)
    {
        var newTool = ToolDbEntity.Create(tag, @event);
        await context.Tools.AddAsync(newTool);
        await context.SaveChangesAsync();
        return newTool;
    }

    public async Task<ToolDbEntity?> DeleteTool(Tag tag, EventBaseDbEntity @event)
    {
        var tool = await GetActiveToolByTagEPCAsync(tag.Id);
        
        if (tool != null)
        {
            tool.Deleted = true;
            tool.DeletedAt = DateTime.UtcNow;
            tool.TargetedEvents.Add(@event);
            await context.SaveChangesAsync();
        }

        return tool;
    }
}