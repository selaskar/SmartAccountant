using System.ComponentModel.DataAnnotations;

namespace SmartAccountant.Repositories.Core.Options;

internal sealed record CoreDatabaseOptions
{
    public const string Section = "CoreDatabase";

    [Required]
    public required string ConnectionString { get; set; }
}
