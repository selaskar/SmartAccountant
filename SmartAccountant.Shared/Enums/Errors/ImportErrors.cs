using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums.Errors;

public enum ImportErrors
{
    [Display(Name = nameof(ModelStrings.ImportErrors_Unspecified), ResourceType = typeof(ModelStrings))]
    Unspecified = 0,
    [Display(Name = nameof(ModelStrings.ImportErrors_UploadedStatementFileEmpty), ResourceType = typeof(ModelStrings))]
    UploadedStatementFileEmpty = 1,
    [Display(Name = nameof(ModelStrings.ImportErrors_UploadedStatementFileTooBig), ResourceType = typeof(ModelStrings))]
    UploadedStatementFileTooBig = 2,
    [Display(Name = nameof(ModelStrings.ImportErrors_UploadedStatementFileTypeNotSupported), ResourceType = typeof(ModelStrings))]
    UploadedStatementFileTypeNotSupported = 3,
    [Display(Name = nameof(ModelStrings.ImportErrors_SavingAccountExpected), ResourceType = typeof(ModelStrings))]
    SavingAccountExpected = 4,
    [Display(Name = nameof(ModelStrings.ImportErrors_CannotCheckExistingTransactions), ResourceType = typeof(ModelStrings))]
    CannotCheckExistingTransactions = 5,
    [Display(Name = nameof(ModelStrings.ImportErrors_AccountDoesNotBelongToUser), ResourceType = typeof(ModelStrings))]
    AccountDoesNotBelongToUser = 6,
    [Display(Name = nameof(ModelStrings.ImportErrors_CannotValidateAccountHolder), ResourceType = typeof(ModelStrings))]
    CannotValidateAccountHolder = 7,
    [Display(Name = nameof(ModelStrings.ImportErrors_CannotSaveUploadedStatementFile), ResourceType = typeof(ModelStrings))]
    CannotSaveUploadedStatementFile = 8,
    [Display(Name = nameof(ModelStrings.ImportErrors_CannotSaveImportedStatement), ResourceType = typeof(ModelStrings))]
    CannotSaveImportedStatement = 9,
    [Display(Name = nameof(ModelStrings.ImportErrors_StatementTypeMismatch), ResourceType = typeof(ModelStrings))]
    StatementTypeMismatch = 10,
    [Display(Name = nameof(ModelStrings.ImportErrors_CannotParseUploadedStatementFile), ResourceType = typeof(ModelStrings))]
    CannotParseUploadedStatementFile = 11,
    [Display(Name = nameof(ModelStrings.ImportErrors_AbstractCreditCardExpected), ResourceType = typeof(ModelStrings))]
    AbstractCreditCardExpected = 12,
    [Display(Name = nameof(ModelStrings.ImportErrors_PrimaryCardNumberNotDetermined), ResourceType = typeof(ModelStrings))]
    PrimaryCardNumberNotDetermined = 13,
    [Display(Name = nameof(ModelStrings.ImportErrors_SecondaryCardNumberNotDetermined), ResourceType = typeof(ModelStrings))]
    SecondaryCardNumberNotDetermined = 14,
    [Display(Name = nameof(ModelStrings.ImportErrors_DiscoveredCardNumbersMismatch), ResourceType = typeof(ModelStrings))]
    DiscoveredCardNumbersMismatch = 15,
    [Display(Name = nameof(ModelStrings.ImportErrors_CannotDetermineSecondaryAccount), ResourceType = typeof(ModelStrings))]
    CannotDetermineSecondaryAccount = 16,
    [Display(Name = nameof(ModelStrings.ImportErrors_TotalDueAmountMismatch), ResourceType = typeof(ModelStrings))]
    TotalDueAmountMismatch = 17
}
