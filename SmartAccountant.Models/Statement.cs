namespace SmartAccountant.Models;

public abstract record class Statement : BaseModel
{
    public Guid AccountId { get; init; }

    public Account? Account { get; set; }

    //TODO: use new Period type.
    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    public IList<StatementDocument> Documents { get; init; } = [];
}

public abstract record class Statement<TTransaction> : Statement where TTransaction : Transaction
{
    public IList<TTransaction> Transactions { get; init; } = [];
}
