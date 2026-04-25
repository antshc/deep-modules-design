// Information Leakage Fix — Pagination Convention
//
// UserRepositoryFixed receives a PageRequest and never computes the offset.
// The 1-based convention and the skip formula are owned entirely by PageRequest.

class UserRepositoryFixed
{
    private readonly List<string> _users = [];

    public List<string> GetPage(PageRequest req) =>
        _users.Skip(req.Skip).Take(req.PageSize).ToList();
}
