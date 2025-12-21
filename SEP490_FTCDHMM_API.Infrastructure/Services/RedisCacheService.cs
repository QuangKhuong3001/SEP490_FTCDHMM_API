using System.Text.Json;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
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

            var parts = key.Split(':');

            for (int i = 1; i <= parts.Length; i++)
            {
                var prefix = string.Join(':', parts.Take(i));
                await _db.SetAddAsync($"__prefix::{prefix}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var indexKey = $"__prefix::{prefix}";
            var keys = await _db.SetMembersAsync(indexKey);

            foreach (var key in keys)
                await _db.KeyDeleteAsync(key.ToString());

            await _db.KeyDeleteAsync(indexKey);
        }

        public async Task HashSetAsync<T>(string key, string field, T value, TimeSpan? ttl = null)
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            await _db.HashSetAsync(key, field, json);

            if (ttl.HasValue)
                await _db.KeyExpireAsync(key, ttl);
        }

        public async Task<List<T>> HashGetAllAsync<T>(string key)
        {
            var entries = await _db.HashGetAllAsync(key);

            return entries
                .Select(e => JsonSerializer.Deserialize<T>(e.Value!, JsonOptions)!)
                .ToList();
        }

        public async Task DeleteKeyAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
