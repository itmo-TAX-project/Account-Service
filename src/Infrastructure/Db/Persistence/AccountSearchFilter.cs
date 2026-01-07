using Infrastructure.Models;

namespace Infrastructure.Db.Persistence;

public record AccountSearchFilter(
    long? Id = null, 
    string? Name = null, 
    string? Phone = null, 
    AccountRole? Role = null, 
    bool? IsBanned = null);