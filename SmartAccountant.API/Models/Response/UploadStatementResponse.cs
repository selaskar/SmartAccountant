namespace SmartAccountant.API.Models.Response;

public record UploadStatementResponse : BaseResponse
{
    public Guid StatementId { get; init; }

    public Guid AccountId { get; init; }
}
