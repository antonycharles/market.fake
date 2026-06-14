using System.ComponentModel.DataAnnotations;

namespace User.Core
{
    public class UserSettings
    {
        [Required]
        public string DatabaseConnection { get; set; }
        [Required]
        public string RedisConnection { get; set; }
        [Required]
        public string FileApiUrl { get; set; }
        [Required]
        public string? RedisInstanceName { get; set; }
        [Required]
        public string AccountsApiUrl { get; set; }
        [Required]
        public string UserApiSlug { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}
