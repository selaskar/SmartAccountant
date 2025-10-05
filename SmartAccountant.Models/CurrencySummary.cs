namespace SmartAccountant.Models;

public record CurrencySummary : BaseModel
{
    public CurrencySummary(Currency currency)
    {
        RemainingBalancesTotal =
            OriginalLimitsTotal =
            IncomeTotal =
            ExpensesTotal =
            PlannedExpensesTotal =
            LoansTotal =
            SavingsTotal =
            Net =
            new MonetaryValue(0, currency);
    }

    public CurrencySummary()
    {
        // For JSON de-/serialization
    }

    public MonetaryValue RemainingBalancesTotal { get; set; }

    public MonetaryValue OriginalLimitsTotal { get; set; }

    public MonetaryValue IncomeTotal { get; set; }

    public MonetaryValue ExpensesTotal { get; set; }

    public MonetaryValue PlannedExpensesTotal { get; set; }

    public MonetaryValue LoansTotal { get; set; }

    public MonetaryValue SavingsTotal { get; set; }

    public MonetaryValue Net { get; set; }

    public ExpenseSummary? ExpensesBreakdown { get; set; }
}
