using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class DebitStatement : Statement
{
    public Currency Currency { get; set; }
}

internal sealed class CreditCardStatement : Statement
{
    [Column(TypeName = "decimal(19, 4)")]
    public decimal TotalDueAmount { get; init; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal? MinimumDueAmount { get; init; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal? TotalFees { get; init; }

    public DateTimeOffset DueDate { get; set; }
}
