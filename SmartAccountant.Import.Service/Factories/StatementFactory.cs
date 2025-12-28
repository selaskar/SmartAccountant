using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service.Factories;

//This factory class isn't strictly necessary, as long as not called by a generic class.
internal class StatementFactory : IStatementFactory
{
    /// <inheritdoc/>
    public Statement Create(AbstractStatementImportModel model, Account account)
    {
        switch (model)
        {
            case DebitStatementImportModel:
                if (account is not SavingAccount savingAccount)
                    throw new ImportException(ImportErrors.SavingAccountExpected, $"Account (type:{account.GetType().Name}) is expected to be a {typeof(SavingAccount).Name}.");

                return new DebitStatement()
                {
                    Id = Guid.NewGuid(),
                    AccountId = model.AccountId,
                    Account = account,
                    Currency = savingAccount.Currency,
                };
            case MultipartStatementImportModel multipartStatementImportModel:
                return new SharedStatement()
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Account = account,
                    RolloverAmount = multipartStatementImportModel.RolloverAmount,
                    TotalPayments = multipartStatementImportModel.TotalPayments,
                    TotalExpenses = multipartStatementImportModel.TotalExpenses,
                    TotalFees = multipartStatementImportModel.TotalFees,
                    TotalDueAmount = multipartStatementImportModel.TotalDueAmount,
                    MinimumDueAmount = multipartStatementImportModel.MinimumDueAmount,
                    DueDate = multipartStatementImportModel.DueDate,
                    RemainingLimit = multipartStatementImportModel.RemainingLimit,
                };
            case CreditCardStatementImportModel creditCardStatementImportModel:
                return new CreditCardStatement()
                {
                    Id = Guid.NewGuid(),
                    AccountId = model.AccountId,
                    Account = account,
                    RolloverAmount = creditCardStatementImportModel.RolloverAmount,
                    TotalPayments = creditCardStatementImportModel.TotalPayments,
                    TotalExpenses = creditCardStatementImportModel.TotalExpenses,
                    TotalFees = creditCardStatementImportModel.TotalFees,
                    TotalDueAmount = creditCardStatementImportModel.TotalDueAmount,
                    MinimumDueAmount = creditCardStatementImportModel.MinimumDueAmount,
                    DueDate = creditCardStatementImportModel.DueDate,
                    RemainingLimit = creditCardStatementImportModel.RemainingLimit,

                };
            default:
                throw new NotImplementedException($"{model.GetType().Name} is not implemented yet.");
        }
    }
}
