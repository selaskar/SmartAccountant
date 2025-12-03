using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Dtos;

[JsonDerivedType(typeof(SavingAccount), typeDiscriminator: "saving")]
[JsonDerivedType(typeof(CreditCard), typeDiscriminator: "creditCard")]
[JsonDerivedType(typeof(VirtualCard), typeDiscriminator: "virtual")]
public abstract class Account : BaseDto
{
    public Guid HolderId { get; init; }

    public Bank Bank { get; init; }

    [StringLength(50)]
    public string? FriendlyName { get; init; }

    public abstract BalanceType NormalBalance { get; }
}
