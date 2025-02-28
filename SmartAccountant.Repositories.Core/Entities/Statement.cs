using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccountant.Repositories.Core.Entities;

internal abstract class Statement
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }

    public virtual IList<Transaction> Transactions { get; set; } = [];

    public virtual IList<StatementDocument> Documents { get; set; } = [];
}
