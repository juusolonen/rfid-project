using System.ComponentModel.DataAnnotations.Schema;
using DataModels.ApiModels;

namespace Database.Entities;

public class UserDbEntity : BaseDbEntity
{
    public long TagIdentifier { get; set; }

    public required string Username { get; set; }
    
    [InverseProperty("Caller")]
    public List<EventBaseDbEntity> CalledEvents { get; set; }
    
    public List<EventBaseDbEntity> TargetedEvents { get; set; }

    public static UserDbEntity Create(Tag tag)
    {
        return new UserDbEntity
        {
            TagIdentifier = tag.Id,
            Username = tag.Name
        };
    }
}