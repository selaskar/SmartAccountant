namespace SmartAccountant.Abstractions.Models.Request;

public class ImportStatementModel
{
    public Guid AccountId { get; init; }

    public required ImportFile File { get; init; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }
}
