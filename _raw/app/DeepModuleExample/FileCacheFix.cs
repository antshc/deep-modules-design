// ── Deep design: FileCache hides all storage and TTL mechanics ────────────────
//
// Interface surface: two methods — Set(key, value, ttl) and TryGet(key, out value).
// Hidden inside: directory management, the two-file layout, the ISO-8601 expiry
// format, stale-entry cleanup, and a lock that serialises concurrent writers.
// Callers treat FileCache as a simple key/value store with a time-to-live.
// Switching from two files to a single JSON envelope, or adding a memory layer,
// only touches FileCache.

class FileCache
{
    private readonly string _dir;
    private readonly object _sync = new();

    public FileCache(string directory) => _dir = directory;

    public void Set(string key, string value, TimeSpan ttl)
    {
        lock (_sync)
        {
            Directory.CreateDirectory(_dir);
            File.WriteAllText(MetaPath(key), DateTime.UtcNow.Add(ttl).ToString("o"));
            File.WriteAllText(DataPath(key), value);
        }
    }

    public bool TryGet(string key, out string value)
    {
        lock (_sync)
        {
            string meta = MetaPath(key), data = DataPath(key);

            if (File.Exists(meta) && File.Exists(data) &&
                DateTime.TryParse(File.ReadAllText(meta), out var expiry) &&
                expiry >= DateTime.UtcNow)
            {
                value = File.ReadAllText(data);
                return true;
            }

            // eviction is the cache's responsibility, not the caller's
            if (File.Exists(meta)) File.Delete(meta);
            if (File.Exists(data)) File.Delete(data);
            value = "";
            return false;
        }
    }

    private string MetaPath(string key) => Path.Combine(_dir, $"{key}.expiry");
    private string DataPath(string key) => Path.Combine(_dir, $"{key}.dat");
}


