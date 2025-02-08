using System.ComponentModel.DataAnnotations;

namespace SmartAccountant.Models.Request;

public record UploadStatementRequest : BaseRequest
{
    public Guid AccountId { get; init; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    [Required] 
    public required IFormFile File { get; init; }
}
