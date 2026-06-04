using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class Store : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserCreatedId { get; set; }
    }
}