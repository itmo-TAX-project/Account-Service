using Application.Models;
using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Npgsql;

namespace Infrastructure.Extensions.Plugin;

public class AccountRoleMappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<AccountRole>("account_roles");
    }
}