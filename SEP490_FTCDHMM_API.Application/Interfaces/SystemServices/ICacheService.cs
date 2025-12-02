namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    namespace SEP490_FTCDHMM_API.Application.Interfaces
    {
        public interface ICacheService
        {
            Task<T?> GetAsync<T>(string key);
            Task SetAsync<T>(string key, T value, TimeSpan ttl);
            Task RemoveByPrefixAsync(string prefix);
        }
    }

}
