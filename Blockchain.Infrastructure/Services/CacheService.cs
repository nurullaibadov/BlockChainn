using Blockchain.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Blockchain.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache) => _cache = cache;

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var json = await _cache.GetStringAsync(key, ct);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, ct);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
            => await _cache.RemoveAsync(key, ct);

        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
            => await _cache.GetStringAsync(key, ct) != null;

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default)
        {
            var cached = await GetAsync<T>(key, ct);
            if (cached != null) return cached;
            var value = await factory();
            await SetAsync(key, value, expiry, ct);
            return value;
        }
    }
}
