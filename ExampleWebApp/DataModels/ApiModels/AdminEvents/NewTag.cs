namespace DataModels.ApiModels;

public class NewTag : TagMessageBase
{
    public override string Action => Constants.ActionType.NewTag;
}