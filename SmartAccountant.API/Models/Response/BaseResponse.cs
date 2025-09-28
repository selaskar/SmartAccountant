namespace SmartAccountant.API.Models.Response;

public record BaseResponse
{
    public Guid RequestId { get; init; }
}
