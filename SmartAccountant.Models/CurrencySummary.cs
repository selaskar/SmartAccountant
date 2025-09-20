namespace SmartAccountant.Models;

public record CurrencySummary : BaseModel
{
    public MonetaryValue RemainingBalancesTotal { get; init; }

    public IEnumerable<(MonetaryValue originalLimit, MonetaryValue? remainingLimit)> RemainingLimits { get; set; } = [];

    public MonetaryValue IncomeTotal { get; init; }

    public MonetaryValue ExpensesTotal { get; set; }

    public MonetaryValue PlannedExpenses { get; init; }

    public MonetaryValue LoansTotal { get; init; }

    public MonetaryValue SavingsTotal { get; init; }

    public MonetaryValue Net { get; init; }
}
