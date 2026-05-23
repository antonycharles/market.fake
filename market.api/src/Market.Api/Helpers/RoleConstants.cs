using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Api.Helpers
{
    public class RoleConstants
    {
        protected const string code = "7";
        protected const string list = "list";
        protected const string create = "create";
        protected const string update = "update";
        protected const string delete = "delete";


        public class MemberRole{
            private const string prefix = "member";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProjectRole{
            private const string prefix = "project";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }
    }
}