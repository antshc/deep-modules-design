# Interface Quality Example — ISessionStore (C#)

Illustrates the formal/informal contract distinction for a well-designed deep interface.

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

## What makes this a good interface

| Aspect | Detail |
|---|---|
| **Formal contract** | `Guid userId` (not `string`), `TimeSpan ttl` (not `int seconds`), `Task<Session?>` (nullable = may not exist) — types encode meaning |
| **Informal contract** | "Create before Get", null-on-expiry, no-op guarantees on `Delete`/`Renew` — all documented in XML comments |
| **Deep module** | 4 methods hide all storage mechanics (Redis, SQL, in-memory). Callers never know the backing store |
| **No unknown unknowns** | A caller knows the full contract from the interface + comments alone — no need to read the implementation |
| **Common-case simplicity** | Create → Get → Delete covers 90% of use; `Renew` is available but not required |

## What to look for in a review

- **Missing informal contract**: if `GetAsync` returned `Session` (non-nullable) but sometimes threw `SessionExpiredException` — that's an undocumented side effect (unknown unknown).
- **Leaky types**: if the interface exposed `RedisKey` or `SqlCommand` in parameter/return types — that's implementation leaking into the formal contract.
- **Over-detailed**: if `CreateAsync` required `(Guid userId, TimeSpan ttl, string redisHost, int dbIndex)` — storage details pollute the interface.
