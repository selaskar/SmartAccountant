namespace SmartAccountant.Models.Request;

public record UploadCreditCardStatementRequest : AbstractUploadStatementRequest
{
    public decimal TotalDueAmount { get; init; }

    public decimal? MinimumDueAmount { get; init; }

    public decimal? TotalFees { get; init; }

    public DateTimeOffset DueDate { get; set; }
}
