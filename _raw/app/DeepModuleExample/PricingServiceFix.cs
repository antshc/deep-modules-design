// Deep Module Fix — File-Backed Cache with TTL
//
// PricingServiceDeep delegates all caching details to FileCache.
// It calls Set and TryGet with no knowledge of the two-file layout,
// the expiry timestamp format, eviction, or directory management.

class PricingServiceDeep
{
    private readonly FileCache _cache;
    public PricingServiceDeep(FileCache cache) => _cache = cache;

    public string? GetCachedPrice(string itemId) =>
        _cache.TryGet(itemId, out var v) ? v : null;

    public void CachePrice(string itemId, string value) =>
        _cache.Set(itemId, value, TimeSpan.FromMinutes(5));
}
