using Application.Kafka.Messages.AdminCreated;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Application.Kafka.Producers;

public interface IAdminCreatedProducer
{
    Task ProduceAsync(IAsyncEnumerable<KafkaProducerMessage<AdminCreatedMessageKey, AdminCreatedMessageValue>> message, CancellationToken cancellationToken);
}