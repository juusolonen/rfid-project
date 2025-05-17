namespace DataModels.ApiModels;

public class UserLogin : BaseMessage
{
   public override string Action => Constants.ActionType.UserLogout; 
}