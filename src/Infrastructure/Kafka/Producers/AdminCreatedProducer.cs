using Application.Kafka.Messages.AdminCreated;
using Application.Kafka.Producers;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Infrastructure.Kafka.Producers;

public class AdminCreatedProducer : IAdminCreatedProducer
{
    private readonly IKafkaMessageProducer<AdminCreatedMessageKey, AdminCreatedMessageValue> _producer;

    public AdminCreatedProducer(IKafkaMessageProducer<AdminCreatedMessageKey, AdminCreatedMessageValue> producer)
    {
        _producer = producer;
    }

    public async Task ProduceAsync(IAsyncEnumerable<KafkaProducerMessage<AdminCreatedMessageKey, AdminCreatedMessageValue>> message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(message, cancellationToken);
    }
}