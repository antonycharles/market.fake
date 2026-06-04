using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class ProductInformation : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public InformationTypeEnum Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}