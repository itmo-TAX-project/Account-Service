using Application.Kafka.Messages.DriverCreated;
using Application.Kafka.Producers;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Infrastructure.Kafka.Producers;

public class DriverCreatedProducer : IDriverCreatedProducer
{
    private readonly IKafkaMessageProducer<DriverCreatedMessageKey, DriverCreatedMessageValue> _producer;

    public DriverCreatedProducer(IKafkaMessageProducer<DriverCreatedMessageKey, DriverCreatedMessageValue> producer)
    {
        _producer = producer;
    }

    public async Task ProduceAsync(IAsyncEnumerable<KafkaProducerMessage<DriverCreatedMessageKey, DriverCreatedMessageValue>> message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(message, cancellationToken);
    }
}