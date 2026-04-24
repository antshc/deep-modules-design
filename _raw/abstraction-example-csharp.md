# C# Example: Good vs. Bad Abstraction

Illustrates the abstraction principles from *A Philosophy of Software Design* §4.3.

---

## Bad abstraction — leaks unimportant details

```csharp
// Caller is forced to know about internal buffer size and flush strategy.
// These details are unimportant to most callers — they pollute the abstraction.
public interface IFileWriter
{
    void OpenFile(string path, int internalBufferSizeBytes);
    void WriteChunk(byte[] data, int offset, int length);
    void FlushBufferToOsCache();   // caller must call this manually
    void SyncOsCacheToStorage();   // caller must call this too — or data may be lost
    void Close();
}
```

**Problem**: the caller must understand buffer management and the two-step flush.  
These are implementation details that should be hidden.

---

## False abstraction — hides important details (dangerous)

```csharp
// Looks simple, but silently delays writes. Data is NOT on disk after Write().
// Callers (e.g. a database) who need durability guarantees will be surprised.
public interface IFileWriter
{
    void Write(string path, byte[] data);  // no mention of async/deferred flush
}
```

**Problem**: the flush behaviour is important for durability-sensitive callers,  
but it is invisible in the interface — a *false* abstraction.

---

## Good abstraction — hides what doesn't matter, exposes what does

```csharp
/// <summary>
/// Writes data to a file.
///
/// Informal (behavior): Writes are buffered for performance. Call
/// <see cref="FlushAsync"/> to guarantee data is persisted to storage —
/// required if the caller needs crash-safety (e.g. a database checkpoint).
/// If <see cref="FlushAsync"/> is not called, data may be lost on process crash.
/// </summary>
public interface IFileWriter : IAsyncDisposable
{
    /// <summary>
    /// Appends data to the file. Buffered — not guaranteed to be on disk.
    /// </summary>
    Task WriteAsync(ReadOnlyMemory<byte> data);

    /// <summary>
    /// Flushes all buffered data to storage. Blocks until the OS confirms the write.
    /// Call this when crash-safety is required.
    /// </summary>
    Task FlushAsync();
}
```

**Why this works**:

| Aspect | Decision |
|---|---|
| Buffer size, chunk management | Hidden — unimportant to callers |
| Deferred flush behaviour | **Exposed** — important for crash-safety; omitting it would be a false abstraction |
| Internal OS cache vs. storage sync | Hidden — one `FlushAsync` covers both steps |
| Minimal surface | 2 methods instead of 5 |

> The key question for every detail: *does the caller need to know this to use the module correctly?*  
> If yes → expose it. If no → hide it.
