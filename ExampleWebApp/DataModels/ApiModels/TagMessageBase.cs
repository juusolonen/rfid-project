namespace DataModels.ApiModels;

public class TagMessageBase : BaseMessage
{
    public required Tag Tag { get; set; }
}