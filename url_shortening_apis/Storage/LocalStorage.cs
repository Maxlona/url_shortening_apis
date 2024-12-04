using System.Collections.Concurrent;
using url_shortening_apis.Models;

namespace url_shortening_apis.Storage
{
    public class LocalStorage : ILocalStorage
    {
        private readonly ConcurrentDictionary<string, UrlModel> _urlMappings = new();

        public bool AddToStorage(string Key, UrlModel Model)
        {
            if (!KeyExists(Key))
            {
               return _urlMappings.TryAdd(Key, Model);
            }
            return false;
        }

        public bool KeyExists(string Key)
        {
            return _urlMappings.ContainsKey(Key);
        }

        public UrlModel GetFromStorage(string Key)
        {
            if (KeyExists(Key))
            {
                _ = _urlMappings.TryGetValue(Key, out var fullUrl);
                return fullUrl;
            }

            return null;
        }
    }
}
