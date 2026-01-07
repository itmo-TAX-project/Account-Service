using Application.Kafka.Messages;
using Infrastructure.Db.Persistence;
using Infrastructure.Models;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace Application.Kafka.InboxHandlers;

public class AccountCreatedInboxHandler : IKafkaInboxHandler<AccountCreatedMessageKey, AccountCreatedMessageValue>
{
    private IAccountRepository _accountRepository;

    public AccountCreatedInboxHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    
    public async ValueTask HandleAsync(IEnumerable<IKafkaInboxMessage<AccountCreatedMessageKey, AccountCreatedMessageValue>> messages, CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            await _accountRepository.AddAccountAsync(new Account(message.Value.Name, message.Value.Phone, message.Value.Role), cancellationToken);
        }
    }
}