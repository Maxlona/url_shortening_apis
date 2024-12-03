using System.Collections.Concurrent;
using url_shortening_apis.Models;

namespace url_shortening_apis.Service
{
    public class UrlShorteningService : IUrlShorteningService
    {
        private readonly ConcurrentDictionary<string, string> _urlMappings = new();

        public async Task<string> ShortenUrl(UrlModel originalUrl)
        {
            int maxTry = 0;
            Uri? uriResult;

            /// validate user url
            if (Uri.TryCreate(originalUrl.OriginalUrl, UriKind.Absolute, out uriResult))
            {
                /// handle collision
                string NewUID = GenerateNewUID();

                if (!_urlMappings.ContainsKey(NewUID))
                {
                    _ = _urlMappings.TryAdd(NewUID, originalUrl.OriginalUrl);
                    return NewUID;
                }
                else
                {
                    int retry = 10;
                    for (int i = 0; i < retry; i++)
                    {
                        if (!_urlMappings.ContainsKey(NewUID))
                        {
                            _ = _urlMappings.TryAdd(NewUID, originalUrl.OriginalUrl);
                            return NewUID;
                        }
                    }
                    //// try maxed out
                    throw new Exception("Was not able to generate a new key");
                }
            }
            else
            {
                throw new Exception("Invalid input url");
            }
        }


        public async Task<string> GetOriginalUrl(string shortUrl)
        {
            if (_urlMappings.TryGetValue(shortUrl, out var originalUrl))
            {
                return await Task.FromResult(originalUrl);
            }

            return await Task.FromResult(string.Empty);
        }

        /// This can run async on a diffrent thread, as a background service to avoid blocking the main thread

        ////////////////// this should generate up to 6,760 billion unique id
        /// 10 chars should generate up to 26*26*10 ^ 10 
        /// small chars abc, ABC, 123 to the power of 10
        private string GenerateNewUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }
    }
}