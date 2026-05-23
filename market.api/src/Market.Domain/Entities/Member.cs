using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Enums;

namespace Market.Domain.Entities
{
    public class Member : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public StatusEnum Status { get; set; }
    }
}