using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class CreditCardStatement : Statement
{
    /// <summary>
    /// Outstanding debt from previous periods.
    /// </summary>
    [Column(TypeName = "decimal(19, 4)")]
    public decimal? RolloverAmount { get; init; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal TotalDueAmount { get; init; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal? MinimumDueAmount { get; init; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal? TotalFees { get; init; }

    public DateTimeOffset DueDate { get; init; }
}
