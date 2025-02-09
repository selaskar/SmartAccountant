using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Factories;

internal class StatementFactory : IStatementFactory
{
    /// <inheritdoc/>
    public Statement Create(ImportStatementModel model, Account account)
    {
        return account.NormalBalance switch
        {
            BalanceType.Debit => new DebitStatement()
            {
                Id = Guid.NewGuid(),
                AccountId = model.AccountId,
                Account = account,
                PeriodStart = model.PeriodStart,
                PeriodEnd = model.PeriodEnd,
                Currency = ((SavingAccount)account).Currency,
            },
            _ => throw new NotImplementedException($"Balance type ({account.NormalBalance}) is not implemented yet.")
        };
    }
}
