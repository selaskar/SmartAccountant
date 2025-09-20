using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class SharedStatement : Statement
{
    public Guid DependentAccountId { get; init; }

    [ForeignKey(nameof(DependentAccountId))]
    public Account? DependentAccount { get; init; }
}
