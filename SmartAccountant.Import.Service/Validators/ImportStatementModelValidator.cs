﻿using FluentValidation;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core;
using SmartAccountant.Import.Service.Resources;

namespace SmartAccountant.Import.Service.Validators;

internal sealed class ImportStatementModelValidator : AbstractValidator<ImportStatementModel>
{
    public ImportStatementModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.RequestId).NotEmpty();

        RuleFor(x => x.AccountId).NotEmpty();

        RuleFor(x => x.File).NotNull().SetValidator(new ImportFileValidator());

        RuleFor(x => x.File.Length).GreaterThan(0).WithMessage(Messages.UploadedStatementFileEmpty)
            .LessThanOrEqualTo(ImportService.MaxFileSize).WithMessage(Messages.UploadedStatementFileTooBig);

        RuleFor(x => x.PeriodStart.Date).ExclusiveBetween(ApplicationDefinitions.EpochStart.ToDateTime(TimeOnly.MinValue), ApplicationDefinitions.EpochEnd.ToDateTime(TimeOnly.MinValue));

        RuleFor(x => x.PeriodEnd.Date).ExclusiveBetween(ApplicationDefinitions.EpochStart.ToDateTime(TimeOnly.MinValue), ApplicationDefinitions.EpochEnd.ToDateTime(TimeOnly.MinValue));

        RuleFor(x => x).Must(x => x.PeriodStart < x.PeriodEnd).WithMessage(Messages.PeriodStartNotEarlierThanPeriodEnd);
    }
}
