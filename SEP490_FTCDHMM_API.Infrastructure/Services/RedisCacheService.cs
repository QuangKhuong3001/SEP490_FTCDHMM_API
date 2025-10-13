using System.Text.Json;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;
using StackExchange.Redis;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly IServer _server;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public RedisCacheService(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
            _server = connection.GetServer(connection.GetEndPoints().First());
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var data = await _db.StringGetAsync(key);
            if (data.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(data!, JsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            await _db.StringSetAsync(key, json, ttl);
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            var keys = _server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Length == 0) return;

            var tasks = keys.Select(k => _db.KeyDeleteAsync(k));
            await Task.WhenAll(tasks);
        }
    }
}
