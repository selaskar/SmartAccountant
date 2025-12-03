using System.Text.Json.Serialization;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Dtos;

[JsonDerivedType(typeof(DebitTransaction), typeDiscriminator: "debit")]
[JsonDerivedType(typeof(CreditCardTransaction), typeDiscriminator: "creditCard")]
public abstract class Transaction : BaseDto
{
    public Guid AccountId { get; set; }

    public Account? Account { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public MonetaryValue Amount { get; set; }

    public required string Description { get; set; }

    public string? PersonalNote { get; set; }

    public TransactionCategory Category { get; set; }
}
