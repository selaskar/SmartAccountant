namespace SmartAccountant.API.Models.Request;

public record UploadMultipartStatementRequest: UploadCreditCardStatementRequest
{
    public Guid DependentAccountId { get; init; }
}
