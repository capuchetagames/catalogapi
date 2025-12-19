using Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CatalogApi.Service;

public class MemCacheService(IMemoryCache cache) : ICacheService
{
    private readonly IMemoryCache _cache = cache;
    
    public object? Get(string key) => _cache.TryGetValue(key, out var cachedValue) ? cachedValue : null;

    public void Set(string key, object value) => _cache.Set(key, value, TimeSpan.FromMinutes(15));

    public void Remove(string key) => _cache.Remove(key);
}