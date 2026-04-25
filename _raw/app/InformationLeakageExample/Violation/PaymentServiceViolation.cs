// Information Leakage Violation — Audit Log Format
//
// PaymentService duplicates the same audit entry format as UserService.
// Switching from UTC to local time, or to a JSON structure, requires touching
// this class even though it has no business caring about the log format.

class PaymentService
{
    private readonly List<string> _auditLog = [];

    public void RefundPayment(int userId, decimal amount)
    {
        _auditLog.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] REFUND by user:{userId}");
    }
}
