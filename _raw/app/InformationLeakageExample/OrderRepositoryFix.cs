// Information Leakage Fix — Pagination Convention
//
// OrderRepositoryFixed receives a PageRequest and never computes the offset.
// The 1-based convention and the skip formula are owned entirely by PageRequest.

class OrderRepositoryFixed
{
    private readonly List<string> _orders = [];

    public List<string> GetPage(PageRequest req) =>
        _orders.Skip(req.Skip).Take(req.PageSize).ToList();
}
