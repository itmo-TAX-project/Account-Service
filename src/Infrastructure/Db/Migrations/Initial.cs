using FluentMigrator;

namespace Infrastructure.Db.Migrations;

[Migration(001)]
public class Initial : Migration
{
    public override void Up()
    {
        Execute.Sql(@"CREATE TYPE role AS ENUM ('passenger', 'driver', 'admin');");
        
        Create.Table("accounts")
            .WithColumn("account_id").AsInt64().PrimaryKey().Identity()
            .WithColumn("account_name").AsString().NotNullable()
            .WithColumn("account_phone").AsString().NotNullable()
            .WithColumn("account_role").AsCustom("role").NotNullable()
            .WithColumn("account_is_banned").AsBoolean().NotNullable()
            .WithColumn("account_created_at").AsDateTimeOffset().NotNullable();

    }

    public override void Down()
    {
        Delete.Table("accounts");
        Execute.Sql("DROP TYPE IF EXISTS role;");
    }
}