using System.Text.Json.Serialization;

namespace DataModels.ApiModels;

public class BaseElement
{
    public required long Id { get; set; }
    
    [JsonPropertyName("tag_id")]
    public required string TagId { get; set; }
    public required string Name{ get; set; }
    
    public int Slot { get; set; }
}