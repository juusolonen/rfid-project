using System.Text.Json.Serialization;

namespace DataModels.ApiModels;

public class Tag : BaseElement
{
    [JsonPropertyName("type")]
    public required TagType TagType { get; set; }
}

public enum TagType
{
    USER,
    TOOL,
}
