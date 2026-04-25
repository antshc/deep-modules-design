// Deep Module Violation — File-Backed Cache with TTL
//
// PricingService duplicates the same caching details as ProductService:
// the same two-file layout, the same expiry parsing logic, the same stale-entry
// eviction, and the same directory-creation call. Any change to the cache
// mechanism (e.g., new timestamp format, different file naming) must be applied
// in every service separately — a direct consequence of information leakage.

class PricingServiceShallow
{
    private const string CacheDir = "cache"; // path convention duplicated

    public string? GetCachedPrice(string itemId)
    {
        // same two-file layout and expiry logic duplicated
        string metaPath = Path.Combine(CacheDir, $"{itemId}.expiry");
        string dataPath = Path.Combine(CacheDir, $"{itemId}.dat");

        if (!File.Exists(metaPath) || !File.Exists(dataPath))
            return null;

        if (!DateTime.TryParse(File.ReadAllText(metaPath), out var expiry) ||
            expiry < DateTime.UtcNow)
        {
            File.Delete(metaPath);
            File.Delete(dataPath);
            return null;
        }
        return File.ReadAllText(dataPath);
    }

    public void CachePrice(string itemId, string value, TimeSpan ttl)
    {
        Directory.CreateDirectory(CacheDir); // directory creation duplicated
        File.WriteAllText(Path.Combine(CacheDir, $"{itemId}.expiry"),
                          DateTime.UtcNow.Add(ttl).ToString("o")); // format duplicated again
        File.WriteAllText(Path.Combine(CacheDir, $"{itemId}.dat"), value);
    }
}
