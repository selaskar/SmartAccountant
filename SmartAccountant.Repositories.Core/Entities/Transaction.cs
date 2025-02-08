using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal abstract class Transaction
{
    public Guid Id { get; set; }

    public Guid StatementId { get; set; }

    public virtual Statement? Statement { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal Amount { get; set; }

    public Currency AmountCurrency { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}
