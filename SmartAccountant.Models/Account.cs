using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Models;

public abstract record class Account : BaseModel
{
    public Guid HolderId { get; init; }

    public Bank Bank { get; init; }

    [StringLength(50)]
    public string? FriendlyName { get; init; }

    public abstract BalanceType NormalBalance { get; }
}
