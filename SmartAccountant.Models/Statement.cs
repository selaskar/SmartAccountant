namespace SmartAccountant.Models;

public abstract class Statement : BaseModel
{
    public Guid AccountId { get; init; }

    public Account? Account { get; set; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    public IList<Transaction> Transactions { get; init; } = [];

    public IList<StatementDocument> Documents { get; init; } = [];
}
