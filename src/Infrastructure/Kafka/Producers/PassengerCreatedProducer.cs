using Application.Kafka.Messages.PassengerCreated;
using Application.Kafka.Producers;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Infrastructure.Kafka.Producers;

public class PassengerCreatedProducer : IPassengerCreatedProducer
{
    private readonly IKafkaMessageProducer<PassengerCreatedMessageKey, PassengerCreatedMessageValue> _producer;

    public PassengerCreatedProducer(IKafkaMessageProducer<PassengerCreatedMessageKey, PassengerCreatedMessageValue> producer)
    {
        _producer = producer;
    }

    public async Task ProduceAsync(IAsyncEnumerable<KafkaProducerMessage<PassengerCreatedMessageKey, PassengerCreatedMessageValue>> message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(message, cancellationToken);
    }
}