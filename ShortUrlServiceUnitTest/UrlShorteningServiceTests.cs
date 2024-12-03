using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using url_shortening_apis.Models;
using url_shortening_apis.Service;
using url_shortening_apis.Storage;

namespace ShortUrlServiceUnitTest
{
    public class UrlShorteningServiceTests
    {
        private readonly IUrlShorteningService _urlShorteningService;
        private readonly ILocalStorage storage;

        public UrlShorteningServiceTests()
        {
            IServiceCollection services = new ServiceCollection();
            storage = Substitute.For<ILocalStorage>();
            _urlShorteningService = new UrlShorteningService(storage);
        }

        [Fact]
        public async Task ShortenUrl_ShouldReturnShortUrl()
        {
            var originalUrl = new UrlModel { LongUrl = "https://example.com" };
            var shortUrl = await _urlShorteningService.ShortenUrl(originalUrl);

            Assert.NotNull(shortUrl);
            Assert.NotEqual(originalUrl.LongUrl, shortUrl);
        }


        [Fact]
        public async Task ShortenUrl_ValidUrl_ReturnsNewUID()
        {
            // Arrange
            var inputUrlModel = new UrlModel { LongUrl = "https://example.com" };
            storage.KeyExists(Arg.Any<string>()).Returns(false);
            
            // Act

            var result = await _urlShorteningService.ShortenUrl(inputUrlModel);
            // remove domain base url
            result = result.Replace("https://localhost:7148/UrlShortener/api/", string.Empty);
            // Assert
            Assert.NotNull(result);
            storage.Received(1).AddToStorage(result, inputUrlModel);
        }


        [Fact]
        public async Task ShortenUrl_InvalidUrl_ThrowsException()
        {
            // Arrange
            var inputUrlModel = new UrlModel { LongUrl = "invalid-url" };
            //  Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _urlShorteningService.ShortenUrl(inputUrlModel));
            Assert.Equal("Invalid input url", exception.Message);
        }


        [Fact]
        public async Task GetOriginalUrl_ShouldThrowExceptionForInvalidShortUrl()
        {
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                _ = await _urlShorteningService.GetOriginalUrl("invalid");
            });

            Assert.Equal("Short url was not found", exception.Message);
        }

        [Fact]
        public async Task ShortenUrl_ShouldThrowExceptionForInvalidUrl()
        {
            var invalidUrl = new UrlModel { LongUrl = "invalid-url" };

            await Assert.ThrowsAsync<Exception>(async () => await _urlShorteningService.ShortenUrl(invalidUrl));
        }
    }
}
