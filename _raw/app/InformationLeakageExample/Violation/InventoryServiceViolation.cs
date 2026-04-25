// Information Leakage Violation — Audit Log Format
//
// InventoryService duplicates the same audit entry format as UserService and
// PaymentService. Any service added later will invent its own variant, silently
// diverging — a predictable consequence of the format not being owned by one class.

class InventoryService
{
    private readonly List<string> _auditLog = [];

    public void WriteOffStock(int userId, string sku)
    {
        _auditLog.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] WRITE_OFF by user:{userId}");
    }
}
