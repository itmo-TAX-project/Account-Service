using Application.Kafka.Messages.PassengerCreated;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Application.Kafka.Producers;

public interface IPassengerCreatedProducer
{
    Task ProduceAsync(IAsyncEnumerable<KafkaProducerMessage<PassengerCreatedMessageKey, PassengerCreatedMessageValue>> message, CancellationToken cancellationToken);
}