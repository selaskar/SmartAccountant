namespace SmartAccountant.API.Models.Request;

public record BaseRequest
{
    public Guid RequestId { get; init; }
}
