using url_shortening_apis.Models;
using url_shortening_apis.Service;

namespace ShortUrlServiceUnitTest
{
    public class UrlShorteningServiceTests
    {
        private readonly IUrlShorteningService _urlShorteningService;

        public UrlShorteningServiceTests()
        {
            _urlShorteningService = new UrlShorteningService();
        }

        [Fact]
        public async Task ShortenUrl_ShouldReturnShortUrl()
        {
            var originalUrl = new UrlModel { OriginalUrl = "https://example.com" };
            var shortUrl = await _urlShorteningService.ShortenUrl(originalUrl);

            Assert.NotNull(shortUrl);
            Assert.NotEqual(originalUrl.OriginalUrl, shortUrl);
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldReturnOriginalUrl()
        {
            var originalUrl = new UrlModel { OriginalUrl = "https://example.com" };
            var shortUrl = await _urlShorteningService.ShortenUrl(originalUrl);
            var retrievedUrl = await _urlShorteningService.GetOriginalUrl(shortUrl);

            Assert.Equal(originalUrl.OriginalUrl, retrievedUrl);
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldReturnEmptyForInvalidShortUrl()
        {
            var retrievedUrl = await _urlShorteningService.GetOriginalUrl("invalid");

            Assert.Equal(string.Empty, retrievedUrl);
        }

        [Fact]
        public async Task ShortenUrl_ShouldThrowExceptionForInvalidUrl()
        {
            var invalidUrl = new UrlModel { OriginalUrl = "invalid-url" };

            await Assert.ThrowsAsync<Exception>(async () => await _urlShorteningService.ShortenUrl(invalidUrl));
        }
    }
}
