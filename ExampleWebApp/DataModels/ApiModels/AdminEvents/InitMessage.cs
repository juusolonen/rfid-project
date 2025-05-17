namespace DataModels.ApiModels;

public class InitMessage : BaseMessage
{
    public override string Action => Constants.ActionType.Init;
}