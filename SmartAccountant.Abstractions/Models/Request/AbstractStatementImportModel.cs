namespace SmartAccountant.Abstractions.Models.Request;

public abstract class AbstractStatementImportModel
{
    public Guid RequestId { get; init; }

    public Guid AccountId { get; init; }

    public required ImportFile File { get; init; }

    //TODO: use new Period type.
    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }
}
