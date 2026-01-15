using Application.Kafka.Messages.AdminCreated;
using Application.Kafka.Messages.DriverCreated;
using Application.Kafka.Messages.PassengerCreated;
using Application.Kafka.Producers;
using Application.Models;
using Application.Persistence;
using FluentMigrator.Runner;
using Infrastructure.Db.Options;
using Infrastructure.Db.Repositories;
using Infrastructure.Kafka.Producers;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, DatabaseConfigOptions options)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = options.Host,
            Port = Convert.ToInt32(options.Port),
            Username = options.Username,
            Password = options.Password,
        };
        string connectionString = builder.ToString();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithMigrationsIn(typeof(ServiceCollectionExtensions).Assembly));

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<AccountRole>("account_role");
        dataSourceBuilder.ConfigureTypeLoading(connector =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        NpgsqlDataSource dataSource = dataSourceBuilder.Build();
        services.AddSingleton(dataSource);

        services.AddSingleton<IAccountRepository, AccountRepository>();

        return services;
    }

    public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddOutboxProducer<DriverCreatedMessageKey, DriverCreatedMessageValue>(configuration);
        collection.AddOutboxProducer<PassengerCreatedMessageKey, PassengerCreatedMessageValue>(configuration);
        collection.AddOutboxProducer<AdminCreatedMessageKey, AdminCreatedMessageValue>(configuration);

        collection.AddSingleton<IPassengerCreatedProducer, PassengerCreatedProducer>();
        collection.AddSingleton<IDriverCreatedProducer, DriverCreatedProducer>();
        collection.AddSingleton<IAdminCreatedProducer, AdminCreatedProducer>();

        return collection;
    }

    internal static IServiceCollection AddOutboxProducer<TKey, TValue>(this IServiceCollection collection, IConfiguration configuration)
    {
        return collection.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Kafka"))
            .AddProducer(b => b
                .WithKey<TKey>()
                .WithValue<TValue>()
                .WithConfiguration(configuration.GetSection("Kafka:Producers:Message"))
                .SerializeKeyWithNewtonsoft()
                .SerializeValueWithNewtonsoft()
                .WithOutbox()));
    }
}