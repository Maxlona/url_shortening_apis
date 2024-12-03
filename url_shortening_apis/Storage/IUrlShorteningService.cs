using url_shortening_apis.Models;

namespace url_shortening_apis.Service
{
    public interface IUrlShorteningService
    {
        Task<string> GetOriginalUrl(string shortUrl);
        Task<string> ShortenUrl(UrlModel originalUrl);
    }
}