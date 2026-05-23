using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Market.Domain.Settings
{
    public class ProjectSettings
    {
        [Required]
        public required string ConnectionString { get; set; }
        [Required]
        public required string AccountsApiUrl { get; set; }
        [Required]
        public required string AccountsApiSlug { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}