namespace DataModels.ApiModels;

public class AdminRead : BaseMessage
{
    public override string Action => Constants.ActionType.AdminRead;
}