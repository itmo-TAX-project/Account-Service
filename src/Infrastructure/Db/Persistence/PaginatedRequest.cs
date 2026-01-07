using Infrastructure.Models;

namespace Infrastructure.Db.Persistence;

public record PaginatedRequest(
    int? PageSize,
    long? PageToken);