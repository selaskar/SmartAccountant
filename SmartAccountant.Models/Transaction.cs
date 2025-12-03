using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Models;

public abstract record class Transaction : BaseModel
{
    public Guid? AccountId { get; set; }

    public Account? Account { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public MonetaryValue Amount { get; set; }

    public required string Description { get; set; }

    public string? PersonalNote { get; set; }

    public TransactionCategory Category { get; set; }


    /// <exception cref="ArgumentNullException"/>
    public MonetaryValue NormalizeBalance(BalanceType expected)
    {
        ArgumentNullException.ThrowIfNull(Account);

        return Amount * (Account.NormalBalance == expected ? 1 : -1);
    }
}
