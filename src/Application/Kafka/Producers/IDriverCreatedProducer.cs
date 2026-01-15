using Application.Kafka.Messages.DriverCreated;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Application.Kafka.Producers;

public interface IDriverCreatedProducer
{
    Task ProduceAsync(IAsyncEnumerable<KafkaProducerMessage<DriverCreatedMessageKey, DriverCreatedMessageValue>> message, CancellationToken cancellationToken);
}