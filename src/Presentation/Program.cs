using Application.Extensions;
using FluentMigrator.Runner;
using Infrastructure.Db.Options;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation;

internal class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json");
        DatabaseConfigOptions? dbOptions = builder.Configuration.GetSection(DatabaseConfigOptions.SectionName).Get<DatabaseConfigOptions>();

        if (dbOptions == null) throw new NullReferenceException("DatabaseConfigOptions is null.");

        builder.Services.AddPersistence(dbOptions);
        builder.Services.AddKafka(builder.Configuration);

        string serviceUrl = builder.Configuration.GetSection("AccountService:BaseAddress").Value ?? "http://localhost:8080/";
        WebApplication app = builder.Build();

        await using ServiceProvider provider = builder.Services.BuildServiceProvider();
        IMigrationRunner migrationRunner = provider.GetRequiredService<IMigrationRunner>();
        migrationRunner.MigrateUp();

        app.Run(serviceUrl);
    }
}