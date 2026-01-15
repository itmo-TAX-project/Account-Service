using Application.Models;
using Application.Persistence;
using Application.Services.Interfaces;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account> GetAccountAsync(long accountId, CancellationToken cancellationToken)
    {
        var filter = new AccountSearchFilter(accountId);
        var pagination = new PaginatedRequest(1, null);

        AccountPaginatedResponse result =
            await _accountRepository.SearchAccountsByFilterAsync(filter, pagination, cancellationToken);

        if (result.Accounts != null)
            return result.Accounts.FirstOrDefault() ?? throw new NullReferenceException("Account not found");
        throw new NullReferenceException("Account not found");
    }

    public async Task<bool> CheckBanStatusAsync(long accountId, CancellationToken cancellationToken)
    {
        var filter = new AccountSearchFilter(accountId);
        var pagination = new PaginatedRequest(1, null);

        AccountPaginatedResponse result =
            await _accountRepository.SearchAccountsByFilterAsync(filter, pagination, cancellationToken);

        if (result.Accounts != null)
        {
            Account? account = result.Accounts.FirstOrDefault();
            if (account != null)
            {
                return account.IsBanned;
            }
        }

        throw new NullReferenceException("Account not found");
    }
}