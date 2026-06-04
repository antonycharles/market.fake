using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
    }
}