// ── Fix: one struct owns the pagination convention ────────────────────────────
//
// PageRequest encapsulates the 1-based convention and exposes a Skip property.
// Repositories receive a PageRequest and never compute the offset themselves.
// Switching to 0-based pages, or capping pageSize, only touches PageRequest.

readonly struct PageRequest
{
    public int Page     { get; }
    public int PageSize { get; }
    public int Skip     => (Page - 1) * PageSize;

    public PageRequest(int page, int pageSize)
    {
        Page     = page;
        PageSize = pageSize;
    }
}

