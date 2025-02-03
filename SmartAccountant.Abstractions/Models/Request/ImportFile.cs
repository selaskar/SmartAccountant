namespace SmartAccountant.Abstractions.Models.Request;

public class ImportFile
{
    public required string FileName { get; init; }

    public required string ContentType { get; init; }

    public long Length { get; init; }

    public required Func<Stream> OpenReadStream { get; init; }

    public string FileExtension => Path.GetExtension(FileName);
}
