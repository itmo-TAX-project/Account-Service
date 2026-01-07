using Infrastructure.Models;

namespace Infrastructure.Db.Persistence;

public record AccountPaginatedResponse(IEnumerable<Account>? Accounts, long? PageToken);