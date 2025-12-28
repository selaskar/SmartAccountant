using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums.Errors;

public enum ParserErrors
{
    [Display(Name = nameof(ModelStrings.ParserErrors_Unspecified), ResourceType = typeof(ModelStrings))]
    Unspecified = 0,
    [Display(Name = nameof(ModelStrings.ParserErrors_UploadedDocumentMissingSheet), ResourceType = typeof(ModelStrings))]
    UploadedDocumentMissingSheet = 1,
    [Display(Name = nameof(ModelStrings.ParserErrors_UnexpectedAmountFormat), ResourceType = typeof(ModelStrings))]
    UnexpectedAmountFormat = 2,
    [Display(Name = nameof(ModelStrings.ParserErrors_InsufficientColumnCount), ResourceType = typeof(ModelStrings))]
    InsufficientColumnCount = 3,
    [Display(Name = nameof(ModelStrings.ParserErrors_TransactionDateColumnMissing), ResourceType = typeof(ModelStrings))]
    TransactionDateColumnMissing = 4,
    [Display(Name = nameof(ModelStrings.ParserErrors_UnexpectedDateFormat), ResourceType = typeof(ModelStrings))]
    UnexpectedDateFormat = 5,
    [Display(Name = nameof(ModelStrings.ParserErrors_TransactionAmountAndTotalExpensesMismatch), ResourceType = typeof(ModelStrings))]
    TransactionAmountAndTotalExpensesMismatch = 6,
    [Display(Name = nameof(ModelStrings.ParserErrors_TransactionAmountAndBalanceMismatch), ResourceType = typeof(ModelStrings))]
    TransactionAmountAndBalanceMismatch = 7,
    [Display(Name = nameof(ModelStrings.ParserErrors_UnexpectedRemainingAmountFormat), ResourceType = typeof(ModelStrings))]
    UnexpectedRemainingAmountFormat = 8,
    [Display(Name = nameof(ModelStrings.ParserErrors_UnexpectedPartCount), ResourceType = typeof(ModelStrings))]
    UnexpectedPartCount = 9,
    [Display(Name = nameof(ModelStrings.ParserErrors_DeflectionTooLarge), ResourceType = typeof(ModelStrings))]
    DeflectionTooLarge = 10,
    [Display(Name = nameof(ModelStrings.ParserErrors_UnexpectedErrorParsingStatement), ResourceType = typeof(ModelStrings))] 
    UnexpectedErrorParsingStatement = 11,
    [Display(Name = nameof(ModelStrings.ParserErrors_UnexpectedErrorParsingSpreadsheet), ResourceType = typeof(ModelStrings))]
    UnexpectedErrorParsingSpreadsheet = 12
}
