using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums.Errors;

public enum TransactionErrors
{
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
