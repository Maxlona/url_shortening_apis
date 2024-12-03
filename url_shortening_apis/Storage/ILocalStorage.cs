using url_shortening_apis.Models;

namespace url_shortening_apis.Storage
{
    public interface ILocalStorage
    {
        bool AddToStorage(string Key, UrlModel Model);
        UrlModel GetFromStorage(string Key);
        bool KeyExists(string Key);
    }
}