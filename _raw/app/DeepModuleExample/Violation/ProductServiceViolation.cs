// Deep Module Violation — File-Backed Cache with TTL
//
// ProductService manages all caching details inline: it knows the two-file
// layout, must parse the expiry timestamp, must evict stale entries itself,
// and must ensure the directory exists before writing. All of this is storage
// mechanism knowledge that belongs in a dedicated cache module, not here.

class ProductServiceShallow
{
    private const string CacheDir = "cache";

    public string? GetCachedProduct(string sku)
    {
        // caller must know the two-file layout and expiry timestamp format
        string metaPath = Path.Combine(CacheDir, $"{sku}.expiry");
        string dataPath = Path.Combine(CacheDir, $"{sku}.dat");

        if (!File.Exists(metaPath) || !File.Exists(dataPath))
            return null;

        // caller is responsible for parsing the expiry and evicting stale entries
        if (!DateTime.TryParse(File.ReadAllText(metaPath), out var expiry) ||
            expiry < DateTime.UtcNow)
        {
            File.Delete(metaPath);
            File.Delete(dataPath);
            return null;
        }
        return File.ReadAllText(dataPath);
    }

    public void CacheProduct(string sku, string value, TimeSpan ttl)
    {
        Directory.CreateDirectory(CacheDir); // caller must ensure the directory exists
        File.WriteAllText(Path.Combine(CacheDir, $"{sku}.expiry"),
                          DateTime.UtcNow.Add(ttl).ToString("o")); // format decision here
        File.WriteAllText(Path.Combine(CacheDir, $"{sku}.dat"), value);
    }
}
