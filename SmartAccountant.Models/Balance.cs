namespace SmartAccountant.Models;

public record Balance : BaseModel
{
    public Guid SavingAccountId { get; init; }

    public SavingAccount? Account { get; init; }

    public MonetaryValue Amount { get; init; }

    public DateTimeOffset AsOf { get; init; }
}
