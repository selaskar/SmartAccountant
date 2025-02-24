using System.ComponentModel.DataAnnotations;

namespace SmartAccountant.Models.Request;

public abstract record class AbstractUploadStatementRequest : BaseRequest
{
    public Guid AccountId { get; init; }

    //TODO: Individual trx dates could be enough? Move to CC model?
    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    [Required]
    public required IFormFile File { get; init; }
}
