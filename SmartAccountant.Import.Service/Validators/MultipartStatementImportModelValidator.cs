using SmartAccountant.Abstractions.Models.Request;

namespace SmartAccountant.Import.Service.Validators;

internal sealed class MultipartStatementImportModelValidator : StatementImportModelValidator<MultipartStatementImportModel>
{
    public MultipartStatementImportModelValidator()
        : base()
    {
        Include(new CreditCardStatementImportModelValidator());
    }
}
