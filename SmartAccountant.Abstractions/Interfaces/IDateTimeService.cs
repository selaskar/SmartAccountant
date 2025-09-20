namespace SmartAccountant.Abstractions.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}
