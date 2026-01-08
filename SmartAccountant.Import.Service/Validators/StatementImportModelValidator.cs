using FluentValidation;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models.Request;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service.Validators;

internal abstract class StatementImportModelValidator<T> : AbstractValidator<T>
    where T : AbstractStatementImportModel
{
    public StatementImportModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.RequestId).NotEmpty();

        RuleFor(x => x.AccountId).NotEmpty();

        RuleFor(x => x.File).NotNull().SetValidator(new ImportFileValidator());

        RuleFor(x => x.File.Length)
            .GreaterThan(0).WithErrorCode(ImportErrors.UploadedStatementFileEmpty)
            .LessThanOrEqualTo(AbstractImportService.MaxFileSize).WithErrorCode(ImportErrors.UploadedStatementFileTooBig);
    }
}
