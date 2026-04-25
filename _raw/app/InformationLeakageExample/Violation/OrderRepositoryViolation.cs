// Information Leakage Violation — Pagination Convention
//
// OrderRepository duplicates the same 1-based pagination convention as
// UserRepository and ProductRepository. Three independent peers each encoding
// the same rule — none of them the authoritative owner.

class OrderRepository
{
    private readonly List<string> _orders = [];

    public List<string> GetPage(int page, int pageSize)
    {
        int skip = (page - 1) * pageSize;   // same convention duplicated again
        return _orders.Skip(skip).Take(pageSize).ToList();
    }
}
