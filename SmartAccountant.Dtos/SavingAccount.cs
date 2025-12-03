using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Dtos;

public class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;

    public Currency Currency { get; set; }

    public required string AccountNumber { get; init; }

    public IList<Balance> Balances { get; private init; } = [];
}
