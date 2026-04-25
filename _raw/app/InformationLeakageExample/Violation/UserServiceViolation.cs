// Information Leakage Violation — Audit Log Format
//
// UserService embeds the audit entry format inline. It knows the timestamp
// format, field order, separator characters, and the UTC convention — details
// that belong in a dedicated AuditLog class, not scattered across services.

class UserService
{
    private readonly List<string> _auditLog = [];

    public void DeactivateUser(int userId)
    {
        _auditLog.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] DEACTIVATE by user:{userId}");
    }
}
