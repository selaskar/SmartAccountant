namespace SmartAccountant.Models.Request;

public record BaseRequest
{
    public Guid RequestId { get; init; }
}
