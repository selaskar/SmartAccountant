namespace SmartAccountant.Models;

public abstract class Account
{
    public Guid Id { get; set; }

    public required string AccountNumber { get; set; }

    public required string FriendlyName { get; set; }

    public abstract BalanceType NormalBalance { get; }
}

public enum BalanceType
{
    Debit = 0,
    Credit = 1,
}

public class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;
}

public class CreditAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;
}
