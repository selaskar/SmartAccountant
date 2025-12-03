namespace SmartAccountant.Dtos.Response;

public record BaseResponse
{
    public Guid RequestId { get; init; }
}
