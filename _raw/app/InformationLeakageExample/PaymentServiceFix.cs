// Information Leakage Fix — Audit Log Format
//
// PaymentServiceFixed delegates audit formatting to AuditLog.
// It calls Record with only the action name and user ID —
// no timestamp format, no separator, no UTC convention.

class PaymentServiceFixed
{
    private readonly AuditLog _audit;
    public PaymentServiceFixed(AuditLog audit) => _audit = audit;

    public void RefundPayment(int userId, decimal amount) => _audit.Record("REFUND", userId);
}
