using System.ComponentModel.DataAnnotations;

namespace url_shortening_apis.Models
{
    public class UrlModel
    {
        [MaxLength(2000)]
        public string OriginalUrl { get; set; }

        /// for expiration if needed
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
