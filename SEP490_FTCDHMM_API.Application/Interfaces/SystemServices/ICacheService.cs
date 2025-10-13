namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    namespace SEP490_FTCDHMM_API.Application.Interfaces
    {
        public interface ICacheService
        {
            Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
            Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);
            Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
        }
    }

}
