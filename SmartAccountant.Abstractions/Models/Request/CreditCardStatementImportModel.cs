namespace SmartAccountant.Abstractions.Models.Request;

public class CreditCardStatementImportModel : AbstractStatementImportModel
{
    public decimal TotalDueAmount { get; init; }

    public decimal? MinimumDueAmount { get; init; }

    public decimal? TotalFees { get; init; }

    public DateTimeOffset DueDate { get; set; }
}
