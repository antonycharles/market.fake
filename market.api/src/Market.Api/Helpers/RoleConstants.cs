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


        public class StoreRole{
            private const string prefix = "stores";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class CategoryRole{
            private const string prefix = "categories";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProductRole{
            private const string prefix = "products";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProductStockRole{
            private const string prefix = "product-stocks";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProductPriceRole{
            private const string prefix = "product-prices";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProductPhotoRole{
            private const string prefix = "product-photos";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProductInformationRole{
            private const string prefix = "product-informations";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }

        public class ProductCategoryRole{
            private const string prefix = "product-categories";
            public const string List = $"{code}-{prefix}-{list}";
            public const string Create = $"{code}-{prefix}-{create}";
            public const string Update = $"{code}-{prefix}-{update}";
            public const string Delete = $"{code}-{prefix}-{delete}";
        }
    }
}
