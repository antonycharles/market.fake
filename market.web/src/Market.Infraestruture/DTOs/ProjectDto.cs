using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Infraestruture.Enums;

namespace Market.Infraestruture.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StatusEnum Status { get; set; }
    }
}