using url_shortening_apis.Models;

namespace url_shortening_apis.Service
{
    public interface IUrlShorteningService
    {
        Task<UrlModel> GetOriginalUrl(string shortUrl);
        Task<string> ShortenUrl(UrlModel inputUrlModel);
    }
}