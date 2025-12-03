using FluentValidation;
using SmartAccountant.Models;
using SmartAccountant.Shared;

namespace SmartAccountant.Services.Validators;

internal sealed class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        //TODO: localization
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.ReferenceNumber)
            .MaximumLength(100);

        RuleFor(x => x.Timestamp)
            .ExclusiveBetween(ApplicationDefinitions.EpochStart.ToDateTime(TimeOnly.MinValue), ApplicationDefinitions.EpochEnd.ToDateTime(TimeOnly.MinValue));

        RuleFor(x => x.Amount.Amount)
            .ExclusiveBetween((decimal)ApplicationDefinitions.MinTransactionAmount, (decimal)ApplicationDefinitions.MaxTransactionAmount);

        RuleFor(x => x.Amount.Currency)
            .IsInEnum();

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.PersonalNote)
            .MaximumLength(500)
            .When(x => x.PersonalNote is not null);

        RuleFor(x => x.Category.Category)
            .IsInEnum();
    }
}
