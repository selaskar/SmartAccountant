using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Dtos;

public abstract class AbstractCreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    public required string CardNumber { get; init; }
}
