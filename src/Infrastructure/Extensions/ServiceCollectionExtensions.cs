using Application.Kafka.Messages.AdminCreated;
using Application.Kafka.Messages.DriverCreated;
using Application.Kafka.Messages.PassengerCreated;
using Application.Kafka.Producers;
using Application.Persistence;
using Infrastructure.Db.Migrations;
using Infrastructure.Db.Options;
using Infrastructure.Db.Repositories;
using Infrastructure.Extensions.Plugin;
using Infrastructure.Kafka.Producers;
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.MessagePersistence;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagePersistence(this IServiceCollection services)
    {
        services.AddUtcDateTimeProvider();
        services.AddSingleton(new Newtonsoft.Json.JsonSerializerSettings());

        services.AddPlatformMessagePersistence(builder => builder
            .WithDefaultPublisherOptions("MessagePersistence:Publisher:Default")
            .UsePostgresPersistence(configurator => configurator.ConfigureOptions("MessagePersistence")));

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddPlatformPersistence(
            persistence => persistence.UsePostgres(
                postgres => postgres.WithConnectionOptions(
                        builder =>
                        {
                            DatabaseConfigOptions options = services.BuildServiceProvider()
                                .GetRequiredService<IOptions<DatabaseConfigOptions>>().Value;

                            builder.Configure(connectionOptions =>
                            {
                                connectionOptions.Host = options.Host;
                                connectionOptions.Port = options.Port;
                                connectionOptions.Database = options.Database;
                                connectionOptions.Username = options.Username;
                                connectionOptions.Password = options.Password;
                                connectionOptions.SslMode = options.SslMode;
                            });
                        })
                    .WithMigrationsFrom(typeof(Initial).Assembly)
                    .WithDataSourcePlugin<AccountRoleMappingPlugin>()
                    .WithDataSourcePlugin<LegacyTimestampBehaviorPlugin>()));

        return services;
    }

    public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddMessagePersistence();

        collection.AddOutboxProducer<DriverCreatedMessageKey, DriverCreatedMessageValue>(configuration, "Kafka:Producers:DriverCreatedMessage");
        collection.AddOutboxProducer<PassengerCreatedMessageKey, PassengerCreatedMessageValue>(configuration, "Kafka:Producers:PassengerCreatedMessage");
        collection.AddOutboxProducer<AdminCreatedMessageKey, AdminCreatedMessageValue>(configuration, "Kafka:Producers:AdminCreatedMessage");

        collection.AddSingleton<IPassengerCreatedProducer, PassengerCreatedProducer>();
        collection.AddSingleton<IDriverCreatedProducer, DriverCreatedProducer>();
        collection.AddSingleton<IAdminCreatedProducer, AdminCreatedProducer>();

        return collection;
    }

    internal static IServiceCollection AddOutboxProducer<TKey, TValue>(this IServiceCollection collection, IConfiguration configuration, string sectionName)
    {
        return collection.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Kafka"))
            .AddProducer(b => b
                .WithKey<TKey>()
                .WithValue<TValue>()
                .WithConfiguration(configuration.GetSection(sectionName))
                .SerializeKeyWithNewtonsoft()
                .SerializeValueWithNewtonsoft()
                .WithOutbox()));
    }
}