﻿using FluentValidation;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core;

namespace SmartAccountant.Import.Service.Validators;

internal sealed class CreditCardStatementImportModelValidator : StatementImportModelValidator<CreditCardStatementImportModel>
{
    public CreditCardStatementImportModelValidator()
        : base()
    {
        RuleFor(x => x.DueDate.Date).ExclusiveBetween(ApplicationDefinitions.EpochStart.ToDateTime(TimeOnly.MinValue), ApplicationDefinitions.EpochEnd.ToDateTime(TimeOnly.MinValue));
    }
}
