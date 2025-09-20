using System.ComponentModel.DataAnnotations;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;

    public Currency Currency { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string AccountNumber { get; set; }

    public IList<Balance> Balances { get; set; } = [];
}
