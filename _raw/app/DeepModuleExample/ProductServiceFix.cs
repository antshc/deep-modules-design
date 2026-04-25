// Deep Module Fix — File-Backed Cache with TTL
//
// ProductServiceDeep delegates all caching details to FileCache.
// It calls Set and TryGet with no knowledge of the two-file layout,
// the expiry timestamp format, eviction, or directory management.

class ProductServiceDeep
{
    private readonly FileCache _cache;
    public ProductServiceDeep(FileCache cache) => _cache = cache;

    public string? GetCachedProduct(string sku) =>
        _cache.TryGet(sku, out var v) ? v : null;

    public void CacheProduct(string sku, string value) =>
        _cache.Set(sku, value, TimeSpan.FromMinutes(10));
}
