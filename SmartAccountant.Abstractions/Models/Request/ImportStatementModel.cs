namespace SmartAccountant.Abstractions.Models.Request;

public class ImportStatementModel
{
    //TODO: Use this as a log/scope property within ImportService.
    public Guid RequestId { get; init; }

    public Guid AccountId { get; init; }

    public required ImportFile File { get; init; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }
}
