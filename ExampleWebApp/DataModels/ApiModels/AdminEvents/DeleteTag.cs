namespace DataModels.ApiModels;

public class DeleteTag : TagMessageBase
{
    public override string Action => Constants.ActionType.DeleteTag;
}