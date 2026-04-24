# C# Example: Good Interface Design

Illustrates the formal/informal contract distinction from *A Philosophy of Software Design*.

```csharp
/// <summary>
/// Manages persistent storage of user sessions.
///
/// Informal (behavior): Sessions expire automatically after the configured TTL.
/// A session must be created with <see cref="CreateAsync"/> before it can be
/// retrieved or updated. Expired sessions return null from <see cref="GetAsync"/>.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// Creates a new session and returns its unique ID.
    /// </summary>
    /// <param name="userId">The authenticated user this session belongs to.</param>
    /// <param name="ttl">How long until the session expires.</param>
    /// <returns>A unique session identifier.</returns>
    Task<string> CreateAsync(Guid userId, TimeSpan ttl);

    /// <summary>
    /// Retrieves a session by ID. Returns null if not found or expired.
    /// </summary>
    Task<Session?> GetAsync(string sessionId);

    /// <summary>
    /// Extends the session's TTL from now. Has no effect on expired sessions.
    /// </summary>
    Task RenewAsync(string sessionId, TimeSpan ttl);

    /// <summary>
    /// Invalidates the session immediately.
    /// Does nothing if the session does not exist or is already expired.
    /// </summary>
    Task DeleteAsync(string sessionId);
}
```

## Why this is a good interface

| Aspect | Example |
|---|---|
| **Formal** | Method signatures, `Task<>` return types, `Guid`/`TimeSpan` parameter types |
| **Informal** | "Create before Get", null-on-expiry behavior, no-op guarantees on `Delete`/`Renew` |
| **Deep module** | Hides all storage mechanics (Redis, SQL, in-memory) behind 4 simple methods |
| **No unknown unknowns** | A caller knows the full contract from comments alone — no need to read the implementation |

The `/// <summary>` XML docs carry the **informal** contract. The signature carries the **formal** one.
