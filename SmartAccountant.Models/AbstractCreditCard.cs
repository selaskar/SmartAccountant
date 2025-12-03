using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Models;

public abstract record class AbstractCreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    public required string CardNumber { get; init; }
}
