using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums.Errors;

public enum StorageErrors
{
    [Display(Name = nameof(ModelStrings.StorageErrors_AzureStorageError), ResourceType = typeof(ModelStrings))]
    AzureStorageError = 1,
    [Display(Name = nameof(ModelStrings.StorageErrors_UploadFailed), ResourceType = typeof(ModelStrings))]
    UploadFailed = 2,
    [Display(Name = nameof(ModelStrings.StorageErrors_FileAlreadyExists), ResourceType = typeof(ModelStrings))]
    FileAlreadyExists = 3,
}
