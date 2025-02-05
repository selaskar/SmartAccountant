namespace SmartAccountant.Abstractions.Models.Request;

public sealed class ImportFile
{
    public required string FileName { get; init; }

    public required string ContentType { get; init; }

    public required Func<Stream> OpenReadStream { get; init; }

    public long Length => OpenReadStream().Length;

    /// <summary>
    /// Returns null for invalid extensions such as 'a. '
    /// </summary>
    public string? FileExtension
    {
        get
        {
            string? extension = Path.GetExtension(FileName);

            if (string.IsNullOrEmpty(extension)
                || extension.Length == 1) //assuming it is just a dot
                return null;

            // Since Path.GetExtension method accepts ". " as a valid extension.
            if (string.IsNullOrWhiteSpace(extension[1..]))
                return null;

            return extension[0] + extension[1..].Trim();
        }
    }
}
