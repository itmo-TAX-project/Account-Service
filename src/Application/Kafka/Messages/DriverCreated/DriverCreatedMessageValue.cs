namespace Application.Kafka.Messages.DriverCreated;

public record DriverCreatedMessageValue(string Name, string Phone, string LicenseNumber);