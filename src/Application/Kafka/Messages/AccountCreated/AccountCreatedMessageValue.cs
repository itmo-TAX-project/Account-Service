using Application.Models;

namespace Application.Kafka.Messages.AccountCreated;

public record AccountCreatedMessageValue(string Name, string Phone, AccountRole Role, string? LicenseNumber);