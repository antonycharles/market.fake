using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Guid StoreId { get; set; }
        public Store Store { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
    }
}