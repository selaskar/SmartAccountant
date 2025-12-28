using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Abstractions.Exceptions;

public enum TransactionErrors
{
    [Display(Name = nameof(ModelStrings.TransactionErrors_Unspecified), ResourceType = typeof(ModelStrings))]
    Unspecified = 0,
    [Display(Name = nameof(ModelStrings.TransactionErrors_AccountNotFound), ResourceType = typeof(ModelStrings))]
    AccountNotFound = 1,
    [Display(Name = nameof(ModelStrings.TransactionErrors_AccountDoesNotBelongToUser), ResourceType = typeof(ModelStrings))]
    AccountDoesNotBelongToUser = 2,
    [Display(Name = nameof(ModelStrings.TransactionErrors_CannotFetchTransactionsOfAccount), ResourceType = typeof(ModelStrings))]
    CannotFetchTransactionsOfAccount = 3,
    [Display(Name = nameof(ModelStrings.TransactionErrors_CannotUpdateDebitTransaction), ResourceType = typeof(ModelStrings))]
    CannotUpdateDebitTransaction = 4,
    [Display(Name = nameof(ModelStrings.TransactionErrors_CannotUpdateCreditCardTransaction), ResourceType = typeof(ModelStrings))]
    CannotUpdateCreditCardTransaction = 5,
}

public enum StorageErrors
{
    [Display(Name = nameof(ModelStrings.StorageErrors_Unspecified), ResourceType = typeof(ModelStrings))]
    Unspecified = 0,
    [Display(Name = nameof(ModelStrings.StorageErrors_AzureStorageError), ResourceType = typeof(ModelStrings))]
    AzureStorageError = 1,
    [Display(Name = nameof(ModelStrings.StorageErrors_UploadFailed), ResourceType = typeof(ModelStrings))]
    UploadFailed = 2,
    [Display(Name = nameof(ModelStrings.StorageErrors_FileAlreadyExists), ResourceType = typeof(ModelStrings))]
    FileAlreadyExists = 3,
}
