using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace SharedModels.Cache
{
    public static class DistributedCacheExtensions
    {
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            return SetAsyncWithCacheOptions(cache, key, value);
        }

        public static Task SetAsyncWithCacheOptions<T>(this IDistributedCache cache, string key, T value,
            TimeSpan? absoluteExpireTime = default, TimeSpan? unusedExpireTime = default)
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(60);
            options.SlidingExpiration = unusedExpireTime ?? TimeSpan.FromMinutes(20);
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
            return cache.SetAsync(key, bytes, options);
        }

        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T value)
        {
            var val = cache.Get(key);
            value = default;
            if (val == null)
            {
                return false;
            }

            value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
            return true;
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                WriteIndented = true,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }
    }
}