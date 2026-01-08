using FluentValidation;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Validators;

internal sealed class ImportFileValidator : AbstractValidator<ImportFile>
{
    public ImportFileValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.FileExtension).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
        RuleFor(x => x.OpenReadStream).NotNull();
    }
}
