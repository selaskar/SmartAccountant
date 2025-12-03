namespace SmartAccountant.Dtos.Request;

public record UploadMultipartStatementRequest: UploadCreditCardStatementRequest
{
    public Guid DependentAccountId { get; init; }
}
