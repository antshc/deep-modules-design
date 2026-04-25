// Information Leakage Violation — Pagination Convention
//
// ProductRepository duplicates the same 1-based pagination convention as
// UserRepository. Any class accidentally missed during a convention change
// will silently return wrong data.

class ProductRepository
{
    private readonly List<string> _products = [];

    public List<string> GetPage(int page, int pageSize)
    {
        int skip = (page - 1) * pageSize;   // same convention duplicated
        return _products.Skip(skip).Take(pageSize).ToList();
    }
}
