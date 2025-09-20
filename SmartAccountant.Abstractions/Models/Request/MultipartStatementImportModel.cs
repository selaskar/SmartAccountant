namespace SmartAccountant.Abstractions.Models.Request;

public record class MultipartStatementImportModel : CreditCardStatementImportModel
{
    public Guid DependentAccountId { get; init; }
}
