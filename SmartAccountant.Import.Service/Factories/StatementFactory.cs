using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service.Factories;

internal class StatementFactory : IStatementFactory
{
    /// <inheritdoc/>
    public IStatement<TTransaction> Create<TTransaction>(AbstractStatementImportModel model, Account account)
        where TTransaction : Transaction
    {
        switch (model)
        {
            case DebitStatementImportModel:
                if (account is not SavingAccount savingAccount)
                    throw new ImportException(ImportErrors.SavingAccountExpected, $"Account (type:{account.GetType().Name}) is expected to be a {typeof(SavingAccount).Name}.");

                return (IStatement<TTransaction>)new DebitStatement()
                {
                    Id = Guid.NewGuid(),
                    AccountId = model.AccountId,
                    Account = account,
                    Currency = savingAccount.Currency,
                };
            case MultipartStatementImportModel multipartStatementImportModel:
                return (IStatement<TTransaction>)new SharedStatement()
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
                return (IStatement<TTransaction>)new CreditCardStatement()
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
