using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccountant.Repositories.Core.Entities;

internal abstract class Statement
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public required Account Account { get; set; }

    public DateTimeOffset PeriodStart { get; set; }

    public DateTimeOffset PeriodEnd { get; set; }

    public virtual IList<Transaction> Transactions { get; set; } = [];

    public virtual IList<StatementDocument> Documents { get; set; } = [];
}
