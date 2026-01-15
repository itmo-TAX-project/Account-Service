using Infrastructure.Db.Persistence;
using Infrastructure.Models;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Db.Repositories;

public class AccountRepository(NpgsqlDataSource dataSource) : IAccountRepository
{
    private readonly NpgsqlDataSource _dataSource = dataSource;

    public async Task<long> AddAccountAsync(Account account, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into accounts (account_name, account_phone, account_role, account_is_banned, account_created_at)
                           values (:name, :phone, :role, :is_banned, :created_at)
                           returning account_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("name", NpgsqlDbType.Text) { Value = account.Name });
        command.Parameters.Add(new NpgsqlParameter("phone", NpgsqlDbType.Text) { Value = account.Phone });
        command.Parameters.Add(new NpgsqlParameter("role", "account_role") { Value = account.Role });
        command.Parameters.Add(new NpgsqlParameter("is_banned", NpgsqlDbType.Boolean) { Value = account.IsBanned });
        command.Parameters.Add(new NpgsqlParameter("created_at", NpgsqlDbType.TimestampTz) { Value = account.CreatedAt });

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result);
    }

    public async Task UpdateAccountAsync(Account account, CancellationToken cancellationToken)
    {
        const string sql = """
                           update accounts set
                           account_name=:name,
                           account_phone=:phone,
                           account_role=:role,
                           account_is_banned=:is_banned,
                           account_created_at:created_at
                           where account_id=:account_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("account_id", NpgsqlDbType.Bigint) { Value = account.Id });
        command.Parameters.Add(new NpgsqlParameter("name", NpgsqlDbType.Text) { Value = account.Name });
        command.Parameters.Add(new NpgsqlParameter("phone", NpgsqlDbType.Text) { Value = account.Phone });
        command.Parameters.Add(new NpgsqlParameter("role", "account_role") { Value = account.Role });
        command.Parameters.Add(new NpgsqlParameter("is_banned", NpgsqlDbType.Boolean) { Value = account.IsBanned });
        command.Parameters.Add(new NpgsqlParameter("created_at", NpgsqlDbType.TimestampTz) { Value = account.CreatedAt });

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<AccountPaginatedResponse> SearchAccountsByFilterAsync(AccountSearchFilter searchFilter, PaginatedRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
                           select * from accounts 
                           where
                               (:account_id is null or account_id = :account_id)
                               and (:name is null or account_name = :name)
                               and (:phone is null or account_phone = :phone)
                               and (:role is null or account_role = :role)
                               and (:is_banned is null or account_is_banned = :is_banned)
                           and account_id > :cursor
                           order by account_id
                           limit :page_size
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("account_id", NpgsqlDbType.Bigint)
        {
            Value = searchFilter.Id.HasValue ? searchFilter.Id.Value : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("name", NpgsqlDbType.Text)
        {
            Value = searchFilter.Name != null ? searchFilter.Name : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("phone", NpgsqlDbType.Text)
        {
            Value = searchFilter.Phone != null ? searchFilter.Phone : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("role", "account_role")
        {
            DataTypeName = "account_role",
            Value = searchFilter.Role.HasValue ? searchFilter.Role.Value : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("is_banned", NpgsqlDbType.Boolean)
        {
            Value = searchFilter.IsBanned.HasValue ? searchFilter.IsBanned.Value : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("cursor", NpgsqlDbType.Bigint) { Value = request.PageToken ?? 0 });
        command.Parameters.Add(new NpgsqlParameter("page_size", NpgsqlDbType.Integer) { Value = request.PageSize ?? 20 });

        NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var accounts = new List<Account>();
        long? lastId = request.PageToken;

        while (await reader.ReadAsync(cancellationToken))
        {
            long accountId = reader.GetInt64(0);
            accounts.Add(new Account(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetFieldValue<AccountRole>(4),
                reader.GetBoolean(5),
                DateTime.SpecifyKind(reader.GetDateTime(6),  DateTimeKind.Utc),
                accountId));

            lastId = accountId;
        }

        return new AccountPaginatedResponse(accounts, lastId);
    }
}