using System.Text.Json;
using DataModels;
using DataModels.ApiModels;
using Microsoft.EntityFrameworkCore;

namespace Database.Entities;

public class ProcessedEventDbEntity : EventBaseDbEntity
{
    public ProcessedData ProcessedData { get; set; }
    public static ProcessedEventDbEntity FromEventBaseDbEntity(EventBaseDbEntity value)
    {
        var root = value.Data.RootElement;
        
        return new ProcessedEventDbEntity
        {
            ReceivedAt = value.ReceivedAt,
            Data = value.Data,
            Processed = true,
            TargetUserId = value.TargetUserId,
            CallerId = value.CallerId,
            Tools = value.Tools,
            ProcessedData = new ProcessedData
            {
                ProcessedAt = DateTime.UtcNow,
                InitiatorName = (string?)GetProperty(root, Constants.User, JsonValueKind.String),
                InitiatorTagEPC = (long?)GetProperty(root, Constants.UserId, JsonValueKind.Number) ?? 0L,
                InitiatorSystemId = (string?)GetProperty(root, Constants.UserTagId, JsonValueKind.String),
                Action = (string?)GetProperty(root, Constants.Action, JsonValueKind.String),
                Tag = (string?)GetProperty(root, Constants.Tag, JsonValueKind.Object),
                TagNames = ExtractTagNames(root) 
            }
        };
    }

    private static object? GetProperty(JsonElement root, string name, JsonValueKind type)
    {
        if (root.TryGetProperty(name, out JsonElement prop))
        {
            var elementType = prop.ValueKind;
            if (elementType != type)
            {
                return null;
            }
            
            return type switch
            {
                JsonValueKind.Null or JsonValueKind.Undefined => null,
                JsonValueKind.Object => prop.GetRawText(),
                JsonValueKind.Number => prop.GetInt64(),
                JsonValueKind.String => prop.GetString(),
                JsonValueKind.True or JsonValueKind.False => prop.GetBoolean(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
        }

        return type switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined or JsonValueKind.Object => null,
            JsonValueKind.Number => 0,
            JsonValueKind.String => string.Empty,
            JsonValueKind.True or JsonValueKind.False => false,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static ProcessedEventDbEntity FaultedFromEvent(BorrowMessage message, EventBaseDbEntity entity, string toolName, Guid? callerId = null)
    {
        return new ProcessedEventDbEntity
        {
            ReceivedAt = DateTime.UtcNow,
            Processed = true,
            Data = entity.Data,
            Faulted = true,
            FaultReason = "not found",
            CallerId = callerId,
            Tools = new List<ToolDbEntity>(),
            ProcessedData = new ProcessedData
            {
                ProcessedAt = DateTime.UtcNow,
                InitiatorName = message.User,
                Action = message.Action,
                TagNames = [toolName]
            }
        };
    }
    
    private static List<string> ExtractTagNames(JsonElement root)
    {
        var tagNames = new List<string>();

        if (root.TryGetProperty(Constants.Tools, out JsonElement tools) && tools.ValueKind == JsonValueKind.Array)
        {
            foreach (var tool in tools.EnumerateArray())
            {
                if (tool.TryGetProperty(Constants.Name, out JsonElement toolName) &&
                    toolName.ValueKind == JsonValueKind.String)
                {
                    tagNames.Add(toolName.GetString());
                }
            }
            return tagNames.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        }

        if (tagNames.Count == 0)
        {
            if (root.TryGetProperty(Constants.Tag, out JsonElement tag) && tag.ValueKind == JsonValueKind.Object)
            {
                if (tag.TryGetProperty(Constants.Name, out JsonElement tagName) &&
                    tagName.ValueKind == JsonValueKind.String)
                {
                    var tagname = tagName.GetString();
                    if (tagname != null)
                    {
                        tagNames.Add(tagname);
                    }
                }
            }
        }
        return tagNames;
    }

}

[Owned]
public class ProcessedData
{
    public DateTime ProcessedAt { get; set; }
    
    // Initiator
    public string? InitiatorName { get; set; }
    
    public long? InitiatorTagEPC { get; set; }
    
    public string? InitiatorSystemId { get; set; }
    
    public string? Action { get; set; }
    
    public string? Tag { get; set; }
    
    // Tool names?
    public List<string>? TagNames { get; set; }
}