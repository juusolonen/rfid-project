namespace DataModels.ApiModels;

public class AdminLogout : BaseMessage
{
    public string Action => Constants.ActionType.AdminLogout;
}