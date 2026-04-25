// Information Leakage Fix — Audit Log Format
//
// UserServiceFixed delegates audit formatting to AuditLog.
// It calls Record with only the action name and user ID —
// no timestamp format, no separator, no UTC convention.

class UserServiceFixed
{
    private readonly AuditLog _audit;
    public UserServiceFixed(AuditLog audit) => _audit = audit;

    public void DeactivateUser(int userId) => _audit.Record("DEACTIVATE", userId);
}
