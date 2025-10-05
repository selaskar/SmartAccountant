using System.ComponentModel.DataAnnotations;

namespace SmartAccountant.API.Models.Request;

public abstract record class AbstractUploadStatementRequest : BaseRequest
{
    public Guid AccountId { get; init; }

    [Required]
    public required IFormFile File { get; init; }
}
