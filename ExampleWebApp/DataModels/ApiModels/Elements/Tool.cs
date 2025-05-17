using System.Text.Json.Serialization;

namespace DataModels.ApiModels;

public class Tool : BaseElement
{
    public bool Out { get; set; }
    
    public bool In { get; set; }
}
