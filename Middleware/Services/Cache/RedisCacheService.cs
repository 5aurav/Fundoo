
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Middleware.Services.Cache
{
    public class RedisCacheService : ICacheService
    {
        private static readonly TimeSpan DefaultTtl =
            TimeSpan.FromMinutes(10);

        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(
            IDistributedCache cache,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await _cache.GetStringAsync(
                    key,
                    cancellationToken);

                if (json == null)
                    return default;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex) when (
                ex is not OperationCanceledException)
            {
                _logger.LogWarning(
                    ex,
                    "Cache read failed for {Key}",
                    key);

                return default;
            }
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? ttl = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        ttl ?? DefaultTtl
                };

                await _cache.SetStringAsync(
                    key,
                    json,
                    options,
                    cancellationToken);
            }
            catch (Exception ex) when (
                ex is not OperationCanceledException)
            {
                _logger.LogWarning(
                    ex,
                    "Cache write failed for {Key}",
                    key);
            }
        }

        public async Task RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(
                    key,
                    cancellationToken);
            }
            catch (Exception ex) when (
                ex is not OperationCanceledException)
            {
                _logger.LogWarning(
                    ex,
                    "Cache removal failed for {Key}",
                    key);
            }
        }
    }
}
