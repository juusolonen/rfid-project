using System.Text.Json.Serialization;

namespace DataModels.ApiModels;

public abstract class BaseMessage
{
    public DateTime Timestamp { get; set; }
    
    public string User { get; set; }
    
    [JsonPropertyName("user_id")]
    public long? UserId { get; set; }
    
    [JsonPropertyName("user_tagid")]
    public string? UserTagId { get; set; }
    
    public virtual string Action { get; set; }
}
