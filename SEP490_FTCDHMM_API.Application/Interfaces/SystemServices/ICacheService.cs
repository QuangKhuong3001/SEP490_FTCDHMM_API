namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        Task RemoveByPrefixAsync(string prefix);

        Task SetAddJsonAsync<T>(string key, T value);
        Task<List<T>> SetMembersJsonAsync<T>(string key);
    }
}
