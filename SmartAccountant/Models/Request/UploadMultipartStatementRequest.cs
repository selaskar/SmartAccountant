namespace SmartAccountant.Models.Request;

public record UploadMultipartStatementRequest: UploadCreditCardStatementRequest
{
    public Guid DependentAccountId { get; init; }
}
