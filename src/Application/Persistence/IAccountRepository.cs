using Application.Models;

namespace Application.Persistence;

public interface IAccountRepository
{
    Task<long> AddAccountAsync(Account account, CancellationToken cancellationToken);

    Task UpdateAccountAsync(Account account, CancellationToken cancellationToken);

    Task<AccountPaginatedResponse> SearchAccountsByFilterAsync(AccountSearchFilter searchFilter, PaginatedRequest request, CancellationToken cancellationToken);
}