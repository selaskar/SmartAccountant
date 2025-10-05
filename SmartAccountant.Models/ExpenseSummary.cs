namespace SmartAccountant.Models;

public record ExpenseSummary
{
    public required IDictionary<ExpenseSubCategories, MonetaryValue>? SubTotals { get; init; }
}
