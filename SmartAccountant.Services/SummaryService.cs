using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;

namespace SmartAccountant.Services;

internal class SummaryService(IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository)
    : ISummaryService
{
    /// <inheritdoc/>
    public async Task<MonthlySummary> GetSummary(DateOnly month, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = authorizationService.UserId;

            // end of month
            var begin = new DateTimeOffset(new DateTime(month.Year, month.Month, 1), TimeSpan.Zero);
            var asOf = new DateTimeOffset(new DateTime(month.Year, month.Month, 1).AddMonths(1), TimeSpan.Zero);

            IEnumerable<Balance> balances = await accountRepository.GetBalancesOfUser(userId, asOf, cancellationToken);

            var currencies = balances.GroupBy(b => b.Account!.Currency,
                (currency, balances) => new MonetaryValue(balances.Sum(x => x.Amount.Amount), currency))
                .ToDictionary(x => x.Currency, mv => new CurrencySummary()
                {
                    RemainingBalancesTotal = mv,
                    PlannedExpenses = new MonetaryValue(0, mv.Currency)
                });

            //TODO: asOf won't yield the correct behavior always.
            IEnumerable<CreditCardLimit> limits = await accountRepository.GetLimitsOfUser(userId, asOf, cancellationToken);

            foreach (IGrouping<Currency, CreditCardLimit> item in limits.GroupBy(l => l.Amount.Currency))
            {
                IEnumerable<(MonetaryValue originalLimit, MonetaryValue? remainingLimit)> remainingLimits = item.Select(l => (l.Amount, (MonetaryValue?)null)).ToList();
                if (currencies.TryGetValue(item.Key, out CurrencySummary? value))
                    value.RemainingLimits = remainingLimits;
                else
                {
                    currencies[item.Key] = new CurrencySummary()
                    {
                        RemainingLimits = remainingLimits,
                    };
                }
            }

            Transaction[] allTransactions = await transactionRepository.GetTransactionsOfUser(userId, begin, asOf, cancellationToken);

            foreach (var item in allTransactions.GroupBy(x => x.Amount.Currency,
                (currency, transactions) => new MonetaryValue(transactions.Sum(x => x.Amount.Amount), currency)))
            {
                if (currencies.TryGetValue(item.Currency, out CurrencySummary? value))
                    value.ExpensesTotal = item;
                else
                {
                    currencies[item.Currency] = new CurrencySummary()
                    {
                        ExpensesTotal = item
                    };
                }
            }

            return new MonthlySummary()
            {
                Id = Guid.NewGuid(),
                Month = month,
                Currencies = currencies.Values.ToList(),
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new SummaryException(Messages.CannotCalculateSummary, ex);
        }
    }
}
