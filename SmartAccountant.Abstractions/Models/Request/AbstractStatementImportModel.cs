namespace SmartAccountant.Abstractions.Models.Request;

public abstract record class AbstractStatementImportModel
{
    public Guid RequestId { get; init; }

    public Guid AccountId { get; init; }

    public required ImportFile File { get; init; }
}
