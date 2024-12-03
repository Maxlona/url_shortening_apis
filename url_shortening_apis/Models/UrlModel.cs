using System.ComponentModel.DataAnnotations;

namespace url_shortening_apis.Models
{
    public class UrlModel
    {
        [Required]
        [MaxLength(2000)]
        [MinLength(10)] /// https://
        public string LongUrl { get; set; }

        /// for expiration if needed
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int ExpirationDays { get; set; } = 0;
    }
}
