using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SmartAccountant.Models;

public abstract record class Account : BaseModel
{
    public Guid HolderId { get; init; }

    public Bank Bank { get; init; }

    [StringLength(50)]
    public string? FriendlyName { get; init; }

    public abstract BalanceType NormalBalance { get; }
}

[SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "We map this enum to a database column.")]
public enum Bank : short
{
    Unknown = 0,
    GarantiBBVA = 1,
}

public enum BalanceType
{
    Debit = 0,
    Credit = 1,
}
