namespace SmartAccountant.Dtos.Request;

public record BaseRequest
{
    public Guid RequestId { get; init; }
}
