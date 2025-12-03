using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Repositories.Core.Entities;

internal abstract class Transaction
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal Amount { get; set; }

    public Currency AmountCurrency { get; set; }

    [StringLength(500)]
    public required string Description { get; set; }

    [StringLength(500)]
    public string? PersonalNote { get; set; }

    public MainCategory Category { get; set; }

    public byte SubCategory { get; set; }
}
