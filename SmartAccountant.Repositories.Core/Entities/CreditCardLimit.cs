using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class CreditCardLimit
{
    public Guid Id { get; set; }

    public Guid CardId { get; set; }

    [ForeignKey(nameof(CardId))]
    public CreditCard? Card { get; set; }

    public DateTimeOffset ValidSince { get; set; }

    public DateTimeOffset ValidUntil { get; set; } 

    [Column(TypeName = "decimal(19, 4)")]
    public decimal Amount { get; set; }

    public Currency AmountCurrency { get; set; }
}
