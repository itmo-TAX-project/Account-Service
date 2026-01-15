using Application.Models;

namespace Application.Persistence;

public record AccountSearchFilter(
    long? Id = null,
    string? Name = null,
    string? Phone = null,
    AccountRole? Role = null,
    bool? IsBanned = null);