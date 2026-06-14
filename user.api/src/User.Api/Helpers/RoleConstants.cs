namespace User.Api.Helpers
{
    public class RoleConstants
    {
        protected const string code = "9";
        protected const string list = "list";
        protected const string create = "create";
        protected const string update = "update";
        protected const string delete = "delete";
        protected const string me = "me";

        public class UserRole
        {
            private const string prefix = "user";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
            public const string MeList = $"{code}-{prefix}-{me}-{list}";
            public const string MeUpdate = $"{code}-{prefix}-{me}-{update}";
        }

        public class UserPhotoRole
        {
            private const string prefix = "photo";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
            public const string MeList = $"{code}-{prefix}-{me}-{list}";
            public const string MeCreate = $"{code}-{prefix}-{me}-{create}";
            public const string MeUpdate = $"{code}-{prefix}-{me}-{update}";
            public const string MeDelete = $"{code}-{prefix}-{me}-{delete}";
        }

        public class UserAddressRole
        {
            private const string prefix = "address";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
            public const string MeList = $"{code}-{prefix}-{me}-{list}";
            public const string MeCreate = $"{code}-{prefix}-{me}-{create}";
            public const string MeUpdate = $"{code}-{prefix}-{me}-{update}";
            public const string MeDelete = $"{code}-{prefix}-{me}-{delete}";
        }

        public class UserCreditCardRole
        {
            private const string prefix = "credit-card";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
            public const string MeList = $"{code}-{prefix}-{me}-{list}";
            public const string MeCreate = $"{code}-{prefix}-{me}-{create}";
            public const string MeUpdate = $"{code}-{prefix}-{me}-{update}";
            public const string MeDelete = $"{code}-{prefix}-{me}-{delete}";
        }
    }
}
