using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class VirtualCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    public Guid ParentId { get; init; }

    [ForeignKey(nameof(ParentId))]
    public Account? Parent { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string CardNumber { get; set; }
}
