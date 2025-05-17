namespace DataModels;

public class Constants
{
    public class ActionType
    {
        public const string AdminRead = "admin_read";
        public const string AdminLogout = "admin_logout";
        public const string NewTag = "newtag";
        public const string DeleteTag = "delete";
        public const string UserLogout = "user_out";
        public const string UserLogin = "user_in";
        public const string Init = "init";
        public const string CreateAdmin = "create_admin";
        public const string Borrow = "borrow";
    }
    
    public static readonly string User = "user";
    public static readonly string UserId = "user_id";
    public static readonly string UserTagId = "user_tagid";
    public static readonly string Action = "action";
    public static readonly string Tag = "tag";
    public static readonly string Tools = "tools";
    public static readonly string Name = "name";
}