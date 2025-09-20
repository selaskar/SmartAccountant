using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;

namespace SmartAccountant.Import.Service.Helpers;

internal class FileTypeValidator : IFileTypeValidator
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

    /// <inheritdoc/>
    public async Task<bool> IsValidFile(ImportFile file, CancellationToken cancellationToken)
    {
        string fileName = file.FileName;
        if (string.IsNullOrEmpty(fileName))
            return false;

        string ext = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(ext) || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(fileName)))
            return false;

        ext = ext.ToUpperInvariant();

        // If we don't have a signature for this file type, we don't confirm its validity.
        if (!fileSignature.TryGetValue(ext, out var tuple))
            return false;

        if (file.ContentType != tuple.contentType)
            return false;

        Stream readStream = file.OpenReadStream();

        int maxSignatureLength = tuple.signature.Max(x => x.Length);
        byte[] fileData = new byte[maxSignatureLength];
        int readBytes = await readStream.ReadAsync(fileData, cancellationToken);

        if (readBytes == 0)
            return false;

        // Reset the stream to its original position.
        readStream.Seek(-readBytes, SeekOrigin.Current);

        // Check if the file signature matches any of the known signatures.
        foreach (byte[] bytes in tuple.signature.Where(sig => sig.Length <= readBytes))
        {
            var curFileSig = new byte[bytes.Length];
            Array.Copy(fileData, curFileSig, bytes.Length);
            if (curFileSig.SequenceEqual(bytes))
                return true;
        }

        return false;
    }
}
