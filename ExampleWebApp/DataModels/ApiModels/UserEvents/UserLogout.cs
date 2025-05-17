namespace DataModels.ApiModels;

public class UserLogout : BaseMessage
{
   public override string Action => Constants.ActionType.UserLogout; 
}