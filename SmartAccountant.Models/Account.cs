using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartAccountant.Models;

[JsonDerivedType(typeof(SavingAccount), typeDiscriminator: "saving")]
[JsonDerivedType(typeof(CreditCard), typeDiscriminator: "creditCard")]
[JsonDerivedType(typeof(VirtualCard), typeDiscriminator: "virtual")]
public abstract record class Account : BaseModel
{
    public Guid HolderId { get; init; }

    public Bank Bank { get; init; }

    [StringLength(50)]
    public string? FriendlyName { get; init; }

    public abstract BalanceType NormalBalance { get; }
}

public enum Bank : short
{
    Unknown = 0,
    GarantiBBVA = 1,
    Enpara = 2,
    IsBankasi = 3,
    Denizbank = 4,
}

public enum BalanceType
{
    Debit = 0,
    Credit = 1,
}
