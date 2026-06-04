using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Domain.Entities
{
    public class ProductStock : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public long AvailableStock { get; set; }
        public long ReservedStock { get; set; }
        public long SoldStock { get; set; }
    }
}