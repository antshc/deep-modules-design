// ── Fix: one class owns the audit entry format ────────────────────────────────
//
// AuditLog encapsulates the format decision entirely.  Services call
// Log(action, userId) and never see a timestamp format, a separator, or the
// UTC convention.  Switching to JSON, adding a correlation ID, or changing the
// timestamp precision only touches AuditLog.

class AuditLog
{
    private readonly List<string> _entries = [];

    public void Record(string action, int userId)
    {
        _entries.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {action} by user:{userId}");
    }

    public IReadOnlyList<string> Entries => _entries;
}

