using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Database.Entities;

[Index(nameof(Processed))]
[Index(nameof(ReceivedAt))]
public class EventBaseDbEntity : BaseDbEntity, IDisposable
{
    public bool Processed { get; set; }
    
    public DateTime ReceivedAt { get; set; }
    
    public bool Faulted { get; set; }
    
    public string? FaultReason { get; set; }
    
    public JsonDocument? Data { get; set; }
    
    public Guid? CallerId { get; set; }

    public UserDbEntity? Caller { get; set; }    
    
    public Guid? TargetUserId { get; set; }

    public UserDbEntity? TargetUser { get; set; }    
    
    public List<ToolDbEntity>? Tools { get; set; }

    public void Dispose()
    {
       Data.Dispose();
    }

    public void SetFaulted(string reason)
    {
        Faulted = true;
        FaultReason = reason;
    }
}