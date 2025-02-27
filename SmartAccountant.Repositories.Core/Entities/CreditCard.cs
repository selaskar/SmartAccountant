using System.ComponentModel.DataAnnotations;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class CreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string CardNumber { get; set; }

    public IList<CreditCardLimit> Limits { get; set; } = [];
}
