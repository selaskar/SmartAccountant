using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Factories;

internal class StatementFactory : IStatementFactory
{
    /// <inheritdoc/>
    public Statement Create(AbstractStatementImportModel model, Account account)
    {
        switch (account.NormalBalance)
        {
            case BalanceType.Debit:

                if (model is not DebitStatementImportModel _)
                    throw new ImportException($"Model (type: {model.GetType().Name}) is expected to be type of {typeof(DebitStatementImportModel).Name}");

                if (account is not SavingAccount savingAccount)
                    throw new ImportException($"Account (type:{account.GetType().Name}) is expected to be a {typeof(SavingAccount).Name}.");

                return new DebitStatement()
                {
                    Id = Guid.NewGuid(),
                    AccountId = model.AccountId,
                    Account = account,
                    Currency = savingAccount.Currency,
                };
            case BalanceType.Credit:

                if (model is not CreditCardStatementImportModel creditCardStatementImportModel)
                    throw new ImportException($"Model (type: {model.GetType().Name}) is expected to be type of {typeof(CreditCardStatementImportModel).Name}");

                if (account is not CreditCard)
                    throw new ImportException($"Account (type:{account.GetType().Name}) is expected to be a {typeof(CreditCard).Name}.");

                return new CreditCardStatement()
                {
                    Id = Guid.NewGuid(),
                    AccountId = model.AccountId,
                    Account = account,
                    RolloverAmount = creditCardStatementImportModel.RolloverAmount,
                    TotalDueAmount = creditCardStatementImportModel.TotalDueAmount,
                    MinimumDueAmount = creditCardStatementImportModel.MinimumDueAmount,
                    TotalFees = creditCardStatementImportModel.TotalFees,
                    DueDate = creditCardStatementImportModel.DueDate,
                };
            default:
                throw new NotImplementedException($"Balance type ({account.NormalBalance}) is not implemented yet.");
        }
    }
}
