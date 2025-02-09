namespace SmartAccountant.Models;

public abstract class Statement : BaseModel
{
    public Guid AccountId { get; init; }

    public Account? Account { get; set; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    public IList<StatementDocument> Documents { get; init; } = [];
}

public abstract class Statement<TTransaction> : Statement where TTransaction : Transaction
{
    public IList<TTransaction> Transactions { get; init; } = [];
}
