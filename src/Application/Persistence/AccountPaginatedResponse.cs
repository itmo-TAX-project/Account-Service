using Application.Models;

namespace Application.Persistence;

public record AccountPaginatedResponse(IEnumerable<Account>? Accounts, long? PageToken);