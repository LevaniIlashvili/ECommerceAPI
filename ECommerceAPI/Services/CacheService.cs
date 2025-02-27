using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerceAPI.Services
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly IDatabase _redisDatabase;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger, IDatabase database)
        {
            _cache = cache;
            _logger = logger;
            _redisDatabase = database;
        }

        public async Task<T?> GetFromCacheAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData != null)
            {
                _logger.LogInformation($"Cache hit for key: {key}");
                _logger.LogInformation(cachedData);
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            else
            {
                _logger.LogInformation($"Cache miss for key: {key}");
                return default;
            }
        }

        public async Task SetCacheAsync<T>(string key, T data, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize<T>(data), options);
            await _redisDatabase.SetAddAsync("keys", key);
            _logger.LogInformation($"Cache set for key: {key} with expiration: {expiration}");
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
            await _redisDatabase.SetRemoveAsync("keys", key);
            _logger.LogInformation($"Cache removed for key: {key}");
        }

        public async Task RemoveCacheByPrefixAsync(string prefix)
        {
            var cacheKeys = await _redisDatabase.SetMembersAsync("keys");
            foreach (var key in cacheKeys)
            {
                if (key.ToString().StartsWith(prefix))
                {
                    await _cache.RemoveAsync(key.ToString());
                    await _redisDatabase.SetRemoveAsync("keys", key);
                    _logger.LogInformation($"Deleted cache for key: {key}");
                }
            }
        }
    }
}
