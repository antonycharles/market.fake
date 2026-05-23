using System;
using System.ComponentModel.DataAnnotations;

namespace Market.Application.DTOs
{
    public class MemberCreateDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
    }
}