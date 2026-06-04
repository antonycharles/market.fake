using System;

namespace Market.Domain.Entities
{
    public class Member : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
