﻿namespace SmartAccountant.Models;

public record CurrencySummary : BaseModel
{
    public MonetaryValue RemainingBalancesTotal { get; set; }

    public MonetaryValue OriginalLimitsTotal { get; set; }

    public MonetaryValue IncomeTotal { get; set; }

    public MonetaryValue ExpensesTotal { get; set; }

    public MonetaryValue PlannedExpenses { get; set; }

    public MonetaryValue LoansTotal { get; set; }

    public MonetaryValue SavingsTotal { get; set; }

    public MonetaryValue Net { get; set; }

    public ExpenseSummary? ExpensesBreakdown { get; set; }
}
