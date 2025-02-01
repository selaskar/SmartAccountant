namespace SmartAccountant.Models;

public class Statement
{
    public Guid Id { get; init; }

    public Guid Account { get; init; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    public IList<Transaction> Transactions { get; init; } = [];
}
