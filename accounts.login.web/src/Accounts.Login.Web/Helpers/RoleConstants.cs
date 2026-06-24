using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Login.Web.Helpers
{
    public class RoleConstants
    {
        protected const string code = "8";
        protected const string show = "show";
        public class HomeRole{
            private const string prefix = "home";
            public const string Show = $"{code}-{prefix}-{show}";
        }
    }
}