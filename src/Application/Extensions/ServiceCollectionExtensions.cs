using Application.Kafka.InboxHandlers;
using Application.Kafka.Messages.AccountCreated;
using Application.Services;
using Application.Services.Interfaces;
using Itmo.Dev.Platform.Kafka.Consumer;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddScoped<IAccountService, AccountService>();

        collection.AddInboxConsumer<AccountCreatedMessageKey, AccountCreatedMessageValue, AccountCreatedInboxHandler>(configuration, "Kafka:Consumers:AccountCreatedMessage");
        return collection;
    }

    internal static IServiceCollection AddInboxConsumer<TKey, TValue, THandler>(this IServiceCollection collection, IConfiguration configuration, string sectionName) where THandler : class, IKafkaInboxHandler<TKey, TValue>
    {
        return collection.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Kafka"))
            .AddConsumer(b => b
                .WithKey<TKey>()
                .WithValue<TValue>()
                .WithConfiguration(configuration.GetSection(sectionName))
                .DeserializeKeyWithNewtonsoft()
                .DeserializeValueWithNewtonsoft()
                .HandleInboxWith<THandler>()));
    }
}