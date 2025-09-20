using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class Balance
{
    public Guid Id { get; set; }

    public Guid SavingAccountId { get; set; }
   
    [ForeignKey(nameof(SavingAccountId))]
    public SavingAccount? SavingAccount { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal Amount { get; set; }

    public Currency AmountCurrency { get; set; }

    public DateTimeOffset AsOf { get; set; }
}
