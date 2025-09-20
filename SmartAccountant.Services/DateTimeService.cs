using SmartAccountant.Abstractions.Interfaces;

namespace SmartAccountant.Services;

internal class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
