using System.Diagnostics.CodeAnalysis;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Models;

public record CurrencySummary : BaseModel
{
    public CurrencySummary(Currency currency)
    {
        RemainingBalancesTotal =
            OriginalLimitsTotal =
            IncomeTotal =
            ExpensesTotal =
            InterestAndFeesTotal =
            PlannedExpensesTotal =
            LoansTotal =
            SavingsTotal =
            Net =
            new MonetaryValue(0, currency);
    }

    public MonetaryValue RemainingBalancesTotal { get; set; }

    public MonetaryValue OriginalLimitsTotal { get; set; }

    public MonetaryValue IncomeTotal { get; set; }

    public MonetaryValue ExpensesTotal { get; set; }

    public MonetaryValue InterestAndFeesTotal { get; set; }

    public MonetaryValue PlannedExpensesTotal { get; set; }

    public MonetaryValue LoansTotal { get; set; }

    public MonetaryValue SavingsTotal { get; set; }

    public MonetaryValue Net { get; set; }

    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This property will not be modified after initial set.")]
    public IDictionary<ExpenseSubCategories, MonetaryValue> ExpensesBreakdown { get; set; } = new Dictionary<ExpenseSubCategories, MonetaryValue>();
}
