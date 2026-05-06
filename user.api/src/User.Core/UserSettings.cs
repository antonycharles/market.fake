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
    }
}
