using Infrastructure.Models;

namespace Application.Kafka.Messages;

public record AccountCreatedMessageValue(string Name, string Phone, AccountRole Role);