// Information Leakage Fix — Audit Log Format
//
// InventoryServiceFixed delegates audit formatting to AuditLog.
// It calls Record with only the action name and user ID —
// no timestamp format, no separator, no UTC convention.

class InventoryServiceFixed
{
    private readonly AuditLog _audit;
    public InventoryServiceFixed(AuditLog audit) => _audit = audit;

    public void WriteOffStock(int userId, string sku) => _audit.Record("WRITE_OFF", userId);
}
