using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartAccountant.Dtos.Request;

public abstract record class AbstractUploadStatementRequest : BaseRequest
{
    public Guid AccountId { get; init; }

    [Required]
    public required IFormFile File { get; init; }
}
