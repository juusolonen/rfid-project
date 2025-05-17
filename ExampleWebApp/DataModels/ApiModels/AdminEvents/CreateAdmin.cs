namespace DataModels.ApiModels;

public class CreateAdmin : BaseMessage
{
    public override string Action => Constants.ActionType.CreateAdmin;
}