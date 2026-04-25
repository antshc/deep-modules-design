// Information Leakage Fix — Pagination Convention
//
// ProductRepositoryFixed receives a PageRequest and never computes the offset.
// The 1-based convention and the skip formula are owned entirely by PageRequest.

class ProductRepositoryFixed
{
    private readonly List<string> _products = [];

    public List<string> GetPage(PageRequest req) =>
        _products.Skip(req.Skip).Take(req.PageSize).ToList();
}
