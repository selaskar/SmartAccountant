namespace SmartAccountant.Models;

public record class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;

    public Currency Currency { get; set; }

    public required string AccountNumber { get; init; }
}
