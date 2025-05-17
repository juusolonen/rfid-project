using System.ComponentModel.DataAnnotations;
using DataModels.ApiModels;

namespace Database.Entities;

public class ToolDbEntity : BaseDbEntity
{
    public long TagIdentifier { get; set; }

    public required string Name { get; set; }
    
    public int Slot {get; set;}
    
    public bool In {get; set;}
    
    public bool Out {get; set;}
    
    [MaxLength(25)]
    public string? LastBorrower {get; set;}
    
    public DateTime? BorrowedAt {get; set;}
    
    public DateTime? ReturnedAt {get; set;}
    
    public List<EventBaseDbEntity> TargetedEvents { get; set; }

    public static ToolDbEntity Create(Tag tag, EventBaseDbEntity @event)
    {
        return new ToolDbEntity
        {
            TagIdentifier = tag.Id,
            Name = tag.Name,
            Slot = tag.Slot,
            In = true,
            Out = false,
            TargetedEvents = [@event]
        };
    }
}