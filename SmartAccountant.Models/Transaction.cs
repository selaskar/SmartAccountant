using System.Text.Json.Serialization;

namespace SmartAccountant.Models;

[JsonDerivedType(typeof(DebitTransaction), typeDiscriminator: "debit")]
[JsonDerivedType(typeof(CreditCardTransaction), typeDiscriminator: "creditCard")]
public abstract record class Transaction : BaseModel
{
    public Guid? AccountId { get; set; }

    public Account? Account { get; set; }

    public string? ReferenceNumber { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public MonetaryValue Amount { get; init; }

    public string? Description { get; init; }

    public string? PersonalNote { get; set; }

    public TransactionCategory Category { get; set; }


    /// <exception cref="ArgumentNullException"/>
    public MonetaryValue NormalizeBalance(BalanceType expected)
    {
        ArgumentNullException.ThrowIfNull(Account);

        return Amount * (Account.NormalBalance == expected ? 1 : -1);
    }

}
