namespace SmartAccountant.Models;

public record class CreditCardStatement : Statement<CreditCardTransaction>
{
    /// <summary>
    /// Outstanding debt from previous periods.
    /// </summary>
    public decimal RolloverAmount { get; init; }

    public decimal TotalPayments { get; init; }

    public decimal TotalExpenses { get; init; }

    /// <summary>
    /// Fees and total interest amount
    /// </summary>
    public decimal TotalFees { get; init; }

    public decimal TotalDueAmount { get; init; }

    public decimal MinimumDueAmount { get; init; }

    public DateTimeOffset DueDate { get; init; }

    public decimal RemainingLimit { get; init; }
}
