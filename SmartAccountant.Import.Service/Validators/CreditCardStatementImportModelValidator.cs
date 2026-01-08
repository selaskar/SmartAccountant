using FluentValidation;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models.Request;
using SmartAccountant.Shared;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service.Validators;

internal sealed class CreditCardStatementImportModelValidator : StatementImportModelValidator<CreditCardStatementImportModel>
{
    public CreditCardStatementImportModelValidator()
        : base()
    {
        RuleFor(x => x.DueDate.Date).ExclusiveBetween(ApplicationDefinitions.EpochStart.ToDateTime(TimeOnly.MinValue), ApplicationDefinitions.EpochEnd.ToDateTime(TimeOnly.MinValue));

        RuleFor(x => x).Must(x => x.RolloverAmount + x.TotalExpenses + x.TotalFees - x.TotalPayments == x.TotalDueAmount)
            .WithErrorCode(ImportErrors.TotalDueAmountMismatch);
    }
}
