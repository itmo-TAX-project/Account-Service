using Application.Kafka.InboxHandlers;
using Application.Kafka.Messages.AccountCreated;
using Application.Kafka.Messages.DriverCreated;
using Application.Kafka.Messages.PassengerCreated;
using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Consumer;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddInboxConsumer<TKey, TValue, THandler>(this IServiceCollection collection, IConfiguration configuration) where THandler : class, IKafkaInboxHandler<TKey, TValue>
    {
        return collection.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Kafka"))
            .AddConsumer(b => b
                .WithKey<TKey>()
                .WithValue<TValue>()
                .WithConfiguration(configuration.GetSection("Kafka:Consumers:Message"))
                .DeserializeKeyWithNewtonsoft()
                .DeserializeValueWithNewtonsoft()
                .HandleInboxWith<THandler>()));
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
    
    public static IServiceCollection AddKafka(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddInboxConsumer<AccountCreatedMessageKey, AccountCreatedMessageValue, AccountCreatedInboxHandler>(configuration);
        collection.AddOutboxProducer<DriverCreatedMessageKey, DriverCreatedMessageValue>(configuration);
        collection.AddOutboxProducer<PassengerCreatedMessageKey, PassengerCreatedMessageValue>(configuration);
        return collection;
    }
}