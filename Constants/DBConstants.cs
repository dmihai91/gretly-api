namespace GretlyStudio.Constants
{
    public class DBCollections
    {
        public static readonly string USER = "User";
        public static readonly string PROJECT = "Project";
        public static readonly string ROLE = "Role";
        public static readonly string COURSE = "Course";
        public static readonly string QUIZ = "Quiz";
        public static readonly string TEMPLATE = "Template";
    }

    public class DBIndexes
    {
        public static readonly string UNIQUE_USER_EMAIL = "unique_User_email";
        public static readonly string UNIQUE_USER_USERNAME = "unique_User_username";
        public static readonly string UNIQUE_USER_FBASE_USER_ID = "unique_User_fBaseUserId";
    }
}