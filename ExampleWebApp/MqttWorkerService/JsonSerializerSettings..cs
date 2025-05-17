using System.Text.Json;
using System.Text.Json.Serialization;
using DataModels.ApiModels;

namespace MqttWorkerService;

public static class JsonSerializerSettings_
{
    public static JsonSerializerOptions GetDefaults()
    {
        var options = new JsonSerializerOptions();
        options.PropertyNameCaseInsensitive = true;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.RespectNullableAnnotations = true;
        options.Converters.Add(new JsonStringEnumConverter<TagType>());
        options.Converters.Add(new FlexibleStringConverter());
        return options;
    }
}