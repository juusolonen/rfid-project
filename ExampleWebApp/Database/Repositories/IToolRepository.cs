using Database.Entities;
using DataModels.ApiModels;

namespace Database.Repositories;

public interface IToolRepository
{
    public Task<ToolDbEntity?> GetActiveToolByTagEPCAsync(long id);
    
    public Task<ToolDbEntity> CreateToolFromTag(Tag tag, EventBaseDbEntity @event);
    
    public Task<ToolDbEntity?> DeleteTool(Tag tag, EventBaseDbEntity @event);

    public Task<ToolDbEntity?> GetToolByNameAndSlotAsync(string name, int slot);

    public Task<List<ToolDbEntity>> GetAllToolsAsync();

    public Task<bool> UpdateToolBorrowInfo(BorrowMessage message, EventBaseDbEntity @event, Guid? callerId = null);

    public Task<(ToolDbEntity?, List<ProcessedEventDbEntity>)> GetToolAsync(Guid id);
}