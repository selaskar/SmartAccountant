using System.ComponentModel.DataAnnotations;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class SavingAccount : Account
{
    public Currency Currency { get; set; }

    public override BalanceType NormalBalance => BalanceType.Debit;

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string AccountNumber { get; set; }
}
