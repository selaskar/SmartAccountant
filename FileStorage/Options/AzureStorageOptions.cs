using System.ComponentModel.DataAnnotations;

namespace FileStorage.Options;

public record AzureStorageOptions
{
    public const string Section = "AzureStorage";

    [Required]
    public required string ServiceAddress { get; set; }
}
