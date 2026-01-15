using Application.Kafka.Messages.AccountCreated;
using Application.Kafka.Messages.AdminCreated;
using Application.Kafka.Messages.DriverCreated;
using Application.Kafka.Messages.PassengerCreated;
using Application.Kafka.Producers;
using Application.Models;
using Application.Persistence;
using Itmo.Dev.Platform.Kafka.Consumer;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Application.Kafka.InboxHandlers;

public class AccountCreatedInboxHandler : IKafkaInboxHandler<AccountCreatedMessageKey, AccountCreatedMessageValue>
{
    private readonly IAccountRepository _accountRepository;

    private readonly IDriverCreatedProducer _driverCreatedProducer;

    private readonly IPassengerCreatedProducer _passengerCreatedProducer;

    private readonly IAdminCreatedProducer _adminCreatedProducer;

    public AccountCreatedInboxHandler(IAccountRepository accountRepository, IDriverCreatedProducer driverCreatedProducer, IPassengerCreatedProducer passengerCreatedProducer, IAdminCreatedProducer adminCreatedProducer)
    {
        _accountRepository = accountRepository;
        _driverCreatedProducer = driverCreatedProducer;
        _passengerCreatedProducer = passengerCreatedProducer;
        _adminCreatedProducer = adminCreatedProducer;
    }

    public async ValueTask HandleAsync(IEnumerable<IKafkaInboxMessage<AccountCreatedMessageKey, AccountCreatedMessageValue>> messages, CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<AccountCreatedMessageKey, AccountCreatedMessageValue> message in messages)
        {
            long id = await _accountRepository.AddAccountAsync(new Account(message.Value.Name, message.Value.Phone, message.Value.Role), cancellationToken);
            switch (message.Value.Role)
            {
                case AccountRole.Driver:
                    var driverCreatedMessageKey = new DriverCreatedMessageKey();
                    var driverCreatedMessageValue = new DriverCreatedMessageValue(message.Value.Name, message.Value.Phone, message.Value.LicenseNumber ?? throw new InvalidOperationException());
                    var driverCreatedMessage = new KafkaProducerMessage<DriverCreatedMessageKey, DriverCreatedMessageValue>(driverCreatedMessageKey, driverCreatedMessageValue);
                    IAsyncEnumerable<KafkaProducerMessage<DriverCreatedMessageKey, DriverCreatedMessageValue>> driverCreatedMessageListAsync = AsyncEnumerableEx.Return(driverCreatedMessage);

                    await _driverCreatedProducer.ProduceAsync(driverCreatedMessageListAsync, cancellationToken);
                    break;
                case AccountRole.Passenger:
                    var passengerCreatedMessageKey = new PassengerCreatedMessageKey();
                    var passengerCreatedMessageValue = new PassengerCreatedMessageValue(message.Value.Name, message.Value.Phone);
                    var passengerCreatedMessage = new KafkaProducerMessage<PassengerCreatedMessageKey, PassengerCreatedMessageValue>(passengerCreatedMessageKey, passengerCreatedMessageValue);
                    IAsyncEnumerable<KafkaProducerMessage<PassengerCreatedMessageKey, PassengerCreatedMessageValue>> passengerCreatedMessageListAsync = AsyncEnumerableEx.Return(passengerCreatedMessage);

                    await _passengerCreatedProducer.ProduceAsync(passengerCreatedMessageListAsync, cancellationToken);
                    break;
                case AccountRole.Admin:
                    var adminCreatedMessageKey = new AdminCreatedMessageKey();
                    var adminCreatedMessageValue = new AdminCreatedMessageValue(id);
                    var adminCreatedMessage = new KafkaProducerMessage<AdminCreatedMessageKey, AdminCreatedMessageValue>(adminCreatedMessageKey, adminCreatedMessageValue);
                    IAsyncEnumerable<KafkaProducerMessage<AdminCreatedMessageKey, AdminCreatedMessageValue>> adminCreatedMessageListAsync = AsyncEnumerableEx.Return(adminCreatedMessage);

                    await _adminCreatedProducer.ProduceAsync(adminCreatedMessageListAsync, cancellationToken);
                    break;
            }
        }
    }
}