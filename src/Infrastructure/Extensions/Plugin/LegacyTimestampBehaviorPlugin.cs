using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Npgsql;

namespace Infrastructure.Extensions.Plugin;

public class LegacyTimestampBehaviorPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.ConfigureTypeLoading(connector =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });
    }
}