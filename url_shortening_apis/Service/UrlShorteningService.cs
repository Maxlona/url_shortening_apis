using url_shortening_apis.Models;
using url_shortening_apis.Storage;

namespace url_shortening_apis.Service
{
    public class UrlShorteningService : IUrlShorteningService
    {
        private readonly ILocalStorage storage;
        public UrlShorteningService(ILocalStorage _storage)
        {
            storage = _storage;
        }

        public async Task<string> ShortenUrl(UrlModel inputUrlModel)
        {
            /// validate user url
            if (Uri.TryCreate(inputUrlModel.LongUrl, UriKind.Absolute, out Uri uriResult))
            {
                /// handle collision
                string NewUID = GenerateNewUID();

                if (!storage.KeyExists(NewUID))
                {
                    storage.AddToStorage(NewUID, inputUrlModel);
                    return NewUID;
                }
                else
                {
                    /// configs retry count
                    IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
                    int? retry = int.Parse(config?.GetSection("retrys:maxRetryCountToGetNewUID").Value);

                    // if config was not found
                    retry = retry ?? 10;

                    for (int i = 0; i < retry; i++)
                    {
                        NewUID = GenerateNewUID();

                        if (!storage.KeyExists(NewUID))
                        {
                            _ = storage.AddToStorage(NewUID, inputUrlModel);
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

        public async Task<UrlModel> GetOriginalUrl(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl))
            {
                return await Task.FromResult<UrlModel>(null);
            }

            var fullUrl = storage.GetFromStorage(shortUrl);

            if (fullUrl == null)
            {
                throw new Exception("Short url was not found");
            }

            // check for expired content
            if (fullUrl.ExpirationDays > 0  && fullUrl.CreatedDate < DateTime.UtcNow)
            {
                if (DateTime.UtcNow < fullUrl.CreatedDate.AddDays(fullUrl.ExpirationDays))
                {
                    return await Task.FromResult(fullUrl);
                }
                else
                {
                    // expired content
                    return await Task.FromResult<UrlModel>(null);
                }
            }

            /// this was a future date, ex: sales campaign (run on 1/1/2026)
            if (fullUrl.CreatedDate > DateTime.UtcNow)
            {
                // expired content
                return await Task.FromResult<UrlModel>(null);
            }

            // just return the url
            return await Task.FromResult(fullUrl);
        }

        /// This can run async on a diffrent thread, as a background service to avoid blocking the main thread

        ////////////////// this should generate up to 1,099,511,627,776 trillion unique UIDs
        /// 0-9 and a-f ^ 10

        private string GenerateNewUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }

    }
}
