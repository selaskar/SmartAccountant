using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Dtos;

public class Balance : BaseDto
{
    public Guid SavingAccountId { get; init; }

    public SavingAccount? Account { get; init; }

    public MonetaryValue Amount { get; init; }

    public DateTimeOffset AsOf { get; init; }
}
