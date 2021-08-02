using FaunaDB.Types;

namespace Gretly.Models
{
    public class PermissionType
    {
        public static readonly string READ = "Read";
        public static readonly string WRITE = "Write";
        public static readonly string MODIFY = "Modify";
        public static readonly string DELETE = "Delete";
    }

    public class Resource
    {
        public static readonly string USER = "user";
        public static readonly string USER_PROFILE = "user_profile";
        public static readonly string PROJECT = "project";
    }

    public class Permission
    {
        [FaunaField("type")]
        public string Type { get; set; }

        [FaunaField("resource")]
        public string Resource { get; set; }

        [FaunaField("value")]
        public int Value { get; set; }

        [FaunaConstructor]
        public Permission(string res, string type, int value)
        {
            Resource = res;
            Type = type;
            Value = value;
        }
    }
}