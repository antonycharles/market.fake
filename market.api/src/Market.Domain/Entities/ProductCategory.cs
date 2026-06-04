using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public int Order { get; set; }
    }
}
