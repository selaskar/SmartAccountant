using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class StatementDocument
{
    [Key]
    public Guid DocumentId { get; set; }

    public Guid StatementId { get; set; }

    [ForeignKey(nameof(StatementId))]
    public required Statement Statement { get; set; }

    public required string FilePath { get; set; }
}
