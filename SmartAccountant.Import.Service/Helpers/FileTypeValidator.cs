using SmartAccountant.Abstractions.Models.Request;

namespace SmartAccountant.Import.Service.Helpers;

internal static class FileTypeValidator
{

    private static readonly Dictionary<string, (string contentType, List<byte[]> signature)> fileSignature = new()
    {
        { ".XLS", ("application/vnd.ms-excel",
            [
                [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1],
                [0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00],
                [0xFD, 0xFF, 0xFF, 0xFF]
            ])
        },
        { ".XLSX", ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            [
                [0x50, 0x4B, 0x03, 0x04]
            ])
        },
    };

    /// <remarks>The method leaves the position of file stream intact.</remarks>
    public static async Task<bool> IsValidFileExtension(ImportFile file, CancellationToken cancellationToken)
    {
        string fileName = file.FileName;
        if (string.IsNullOrEmpty(fileName))
            return false;

        string ext = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(ext))
            return false;

        ext = ext.ToUpperInvariant();

        // If we don't have a signature for this file type, so we assume it's valid.
        if (!fileSignature.TryGetValue(ext, out var value))
            return true;

        if (file.ContentType != value.contentType)
            return false;

        if (value.signature.Count == 0)
            return true;

        Stream readStream = file.OpenReadStream();

        int maxSignatureLength = value.signature.Max(x => x.Length);
        byte[] fileData = new byte[maxSignatureLength];
        int readBytes = await readStream.ReadAsync(fileData, cancellationToken);

        if (readBytes == 0)
            return false;

        // Reset the stream to its original position.
        readStream.Seek(-readBytes, SeekOrigin.Current);

        // Check if the file signature matches any of the known signatures.
        foreach (byte[] bytes in value.signature.Where(sig => sig.Length <= readBytes))
        {
            var curFileSig = new byte[bytes.Length];
            Array.Copy(fileData, curFileSig, bytes.Length);
            if (curFileSig.SequenceEqual(bytes))
                return true;
        }

        return false;
    }
}
