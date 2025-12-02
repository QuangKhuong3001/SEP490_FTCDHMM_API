using System.Text.Json;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;
using StackExchange.Redis;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RedisCacheService(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _db.StringGetAsync(key);
            if (data.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(data!, JsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            await _db.StringSetAsync(key, json, ttl);

            var prefix = key.Contains(':') ? key.Split(':')[0] : key;
            await _db.SetAddAsync($"__prefix::{prefix}", key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var indexKey = $"__prefix::{prefix}";
            var keys = await _db.SetMembersAsync(indexKey);

            foreach (var key in keys)
            {
                await _db.KeyDeleteAsync(key.ToString());
            }

            await _db.KeyDeleteAsync(indexKey);
        }
    }

}
