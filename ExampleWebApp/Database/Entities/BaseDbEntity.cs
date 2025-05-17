namespace Database.Entities;


public abstract class BaseDbEntity 
{
    public Guid Id { get; set; }
    
    public bool Deleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
}