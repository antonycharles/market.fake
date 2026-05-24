using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Web.DTOs
{
    public class MemberCreateDto
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
    }
}