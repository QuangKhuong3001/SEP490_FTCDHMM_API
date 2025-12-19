namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        Task RemoveByPrefixAsync(string prefix);

        Task HashSetAsync<T>(string key, string field, T value, TimeSpan? ttl = null);
        Task<List<T>> HashGetAllAsync<T>(string key);
        Task DeleteKeyAsync(string key);

    }
}
