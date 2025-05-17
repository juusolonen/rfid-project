using DataModels.ApiModels;

namespace DataModels.Models;

public class HandleEventResponse
{
    public bool Success { get; set; }
    
    public string FaultReason { get; set; }
    
    /// <summary>
    /// Used for User id, not intended for Tools.
    /// </summary>
    public Guid? TargetId { get; set; }
    
    
    public Guid? CallerId { get; set; }
    
    public TagType? TagType { get; set; }
    
    // Find out what this does :)
    public bool HackParameter { get; set; }
}