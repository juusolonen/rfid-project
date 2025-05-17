namespace DataModels.ApiModels;

public class BorrowMessage : BaseMessage
{
    public override string Action => Constants.ActionType.Borrow;
    
    public required List<Tool> Tools { get; set; }
}