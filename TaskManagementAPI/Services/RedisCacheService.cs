
using StackExchange.Redis;
using System.Text.Json;

namespace TaskManagementAPI.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }

        public RedisCacheService()
        {
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, serializedValue, expiry);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var data = await _cache.StringGetAsync(key);
            return data.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(data);
        }
    }
}
