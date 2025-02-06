namespace SmartAccountant.Models;

public class CreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    public required string CardNumber { get; init; }

    //TODO: limit with validity period
}
