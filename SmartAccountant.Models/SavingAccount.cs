namespace SmartAccountant.Models;

public class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;

    public required string AccountNumber { get; init; }
}
