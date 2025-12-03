using FluentValidation;
using SmartAccountant.Models;
using SmartAccountant.Shared;

namespace SmartAccountant.Services.Validators;

internal sealed class DebitTransactionValidator : AbstractValidator<DebitTransaction>
{
    public DebitTransactionValidator() : base()
    {
        //TODO: localization
        Include(new TransactionValidator());

        RuleFor(x => x.RemainingBalance.Currency)
            .IsInEnum()
            .Equal(x => x.Amount.Currency);

        RuleFor(x => x.RemainingBalance.Amount)
            .ExclusiveBetween((decimal)ApplicationDefinitions.MinTransactionAmount, (decimal)ApplicationDefinitions.MaxTransactionAmount);
    }
}
