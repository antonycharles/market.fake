using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class ProductPhoto : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public string FileId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public ProductPhotoEnum Type { get; set; }
    }
}