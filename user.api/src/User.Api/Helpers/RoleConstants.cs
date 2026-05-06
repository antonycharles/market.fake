namespace User.Api.Helpers
{
    public class RoleConstants
    {
        protected const string code = "1";
        protected const string list = "list";
        protected const string create = "create";
        protected const string update = "update";
        protected const string delete = "delete";

        public class UserRole
        {
            private const string prefix = "user";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }
    }
}
