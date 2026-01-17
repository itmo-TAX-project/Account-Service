using Application.Models;

namespace Application.Services.Interfaces;

public interface IAccountService
{
    Task<Account> GetAccountAsync(long accountId, CancellationToken cancellationToken);

    Task<bool> CheckBanStatusAsync(long accountId, CancellationToken cancellationToken);

    Task<long> GetAccountIdByNameAsync(string name, CancellationToken cancellationToken);
}