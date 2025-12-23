using System.Linq.Expressions;
using System.Reflection;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

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

            Dictionary<Currency, CurrencySummary> currencies = new();

            await CalculateOriginalLimits(month, userId, currencies, cancellationToken);

            Transaction[] allTransactions = await transactionRepository.GetTransactionsOfMonth(userId, month, cancellationToken);

            Calculate(currencies, allTransactions, cs => cs.IncomeTotal, BalanceType.Debit, MainCategory.Income);
            Calculate(currencies, allTransactions, cs => cs.ExpensesTotal, BalanceType.Credit, MainCategory.Expense);
            Calculate(currencies, allTransactions, cs => cs.LoansTotal, BalanceType.Debit, MainCategory.Loan);
            Calculate(currencies, allTransactions, cs => cs.SavingsTotal, BalanceType.Debit, MainCategory.Saving);
            Calculate(currencies, allTransactions, cs => cs.InterestAndFeesTotal, BalanceType.Credit, MainCategory.InterestOrFee);

            CalculateSubExpenses(currencies, allTransactions);

            CalculateRemainingBalances(currencies, allTransactions);

            foreach (var currencySummary in currencies.Values)
                currencySummary.Net = currencySummary.IncomeTotal - (currencySummary.ExpensesTotal + currencySummary.InterestAndFeesTotal) + currencySummary.LoansTotal + currencySummary.SavingsTotal;

            return new MonthlySummary()
            {
                Id = Guid.NewGuid(),
                Month = month,
                Currencies = currencies.Values.ToList(),
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new SummaryException(SummaryErrors.CannotCalculateSummary, ex);
        }
    }

    private static void CalculateSubExpenses(Dictionary<Currency, CurrencySummary> currencies, Transaction[] allTransactions)
    {
        foreach (IGrouping<Currency, Transaction> item in allTransactions
            .Where(t => t.Category.Category == MainCategory.Expense)
            .GroupBy(t => t.Amount.Currency))
        {
            if (!currencies.TryGetValue(item.Key, out CurrencySummary? summary))
            {
                summary = currencies[item.Key] = new CurrencySummary(item.Key)
                {
                    Id = Guid.NewGuid(),
                };
            }
            
            summary.ExpensesBreakdown = item.GroupBy(t => (ExpenseSubCategories)t.Category.SubCategory)
                 .ToDictionary(t => t.Key, g => g.Select(x => x.NormalizeBalance(BalanceType.Credit)).Sum());
        }
    }

    private async Task CalculateOriginalLimits(DateOnly month, Guid userId, Dictionary<Currency, CurrencySummary> currencies, CancellationToken cancellationToken)
    {
        var asOf = new DateTimeOffset(new DateTime(month.Year, month.Month, 1).AddMonths(1), TimeSpan.Zero);
        IEnumerable<CreditCardLimit> limits = await accountRepository.GetLimitsOfUser(userId, asOf, cancellationToken);

        foreach (IGrouping<Currency, CreditCardLimit> item in limits.GroupBy(l => l.Amount.Currency))
        {
            if (!currencies.TryGetValue(item.Key, out CurrencySummary? summary))
            {
                summary = currencies[item.Key] = new CurrencySummary(item.Key)
                {
                    Id = Guid.NewGuid(),
                };
            }

            summary.OriginalLimitsTotal = new MonetaryValue(item.Sum(x => x.Amount.Amount), item.Key);
        }
    }

    /// <exception cref="ArgumentException"/>
    private static void Calculate(Dictionary<Currency, CurrencySummary> currencies, IEnumerable<Transaction> allTransactions,
        Expression<Func<CurrencySummary, MonetaryValue>> propertySelector,
        BalanceType balanceType,
        params MainCategory[] categories)
    {
        IEnumerable<MonetaryValue> filtered = allTransactions
                .Where(t => categories.Contains(t.Category.Category))
                .GroupBy(x => x.Amount.Currency, (currency, transactions) => transactions.Select(x => x.NormalizeBalance(balanceType)).Sum());

        var propertyInfo = (propertySelector.Body as MemberExpression)?.Member as PropertyInfo ??
            throw new ArgumentException("Property selector must target a property.");

        foreach (MonetaryValue item in filtered)
        {
            if (!currencies.TryGetValue(item.Currency, out CurrencySummary? summary))
            {
                summary = currencies[item.Currency] = new CurrencySummary(item.Currency)
                {
                    Id = Guid.NewGuid(),
                };
            }

            propertyInfo.SetValue(summary, item);
        }
    }

    private static void CalculateRemainingBalances(Dictionary<Currency, CurrencySummary> currencies, IEnumerable<Transaction> allTransactions)
    {
        //TODO: for saving accounts that have no transaction within that month, we need to search past.
        IEnumerable<MonetaryValue> remainingBalances = allTransactions
            .OfType<DebitTransaction>()
            .GroupBy(x => x.AccountId, (accountId, debits) => debits.OrderByDescending(x => x.Timestamp).ThenByDescending(x => x.ReferenceNumber).First())
            .GroupBy(x => x.RemainingBalance.Currency, (currency, debits) => new MonetaryValue(debits.Sum(d => d.RemainingBalance.Amount), currency));

        foreach (MonetaryValue item in remainingBalances)
        {
            if (!currencies.TryGetValue(item.Currency, out CurrencySummary? summary))
            {
                summary = currencies[item.Currency] = new CurrencySummary(item.Currency)
                {
                    Id = Guid.NewGuid(),
                };
            }

            summary.RemainingBalancesTotal = item;
        }
    }
}
