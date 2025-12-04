using FluentValidation;
using SmartAccountant.Models;

namespace SmartAccountant.Services.Validators;

internal sealed class CreditCardTransactionValidator : AbstractValidator<CreditCardTransaction>
{
    public CreditCardTransactionValidator() : base()
    {
        //TODO: localization
        Include(new TransactionValidator());

        RuleFor(x => x.ProvisionState)
            .IsInEnum();
    }
}
