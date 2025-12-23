namespace SmartAccountant.Dtos.Response;

public class ErrorDetail
{
    public int Code { get; set; }

    public required string Error { get; set; }

    public string? Detail { get; set; }

    public ErrorCategory Category { get; set; }
}

public enum ErrorCategory
{
    ServerError,
    ValidationException,
    EnumException
}
