using DataModels.ApiModels;

namespace DataModels.Utility;

public static class MessageTypeResolver
{
    public static Type GetModelType(string actionType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actionType);
        
        return actionType switch
        {
            Constants.ActionType.NewTag => typeof(NewTag),
            Constants.ActionType.DeleteTag => typeof(DeleteTag),
            Constants.ActionType.AdminRead => typeof(AdminRead),
            Constants.ActionType.AdminLogout => typeof(AdminLogout),
            Constants.ActionType.UserLogout => typeof(UserLogout),
            Constants.ActionType.UserLogin => typeof(UserLogin),
            Constants.ActionType.Init => typeof(InitMessage),
            Constants.ActionType.CreateAdmin => typeof(CreateAdmin),
            Constants.ActionType.Borrow => typeof(BorrowMessage),
            _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
        };
    }
}