// Information Leakage Violation — Pagination Convention
//
// UserRepository hardcodes the 1-based pagination convention inline.
// If the convention changes to 0-based pages or cursor-based pagination,
// this class must be updated separately from every other repository.

class UserRepository
{
    private readonly List<string> _users = [];

    public List<string> GetPage(int page, int pageSize)
    {
        int skip = (page - 1) * pageSize;   // pagination convention hardcoded here
        return _users.Skip(skip).Take(pageSize).ToList();
    }
}
