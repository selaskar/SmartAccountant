using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service;

internal abstract class AbstractCreditCardImportService<TTransaction>(
    ILogger<AbstractCreditCardImportService<TTransaction>> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStatementFactory statementFactory,
    IStatementParser parser,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    IStatementRepository statementRepository,
    ITransactionRepository transactionRepository,
    IDateTimeService dateTimeService)
    : AbstractImportService<TTransaction>(logger, fileTypeValidator, authorizationService, accountRepository, statementFactory, parser, storageService, unitOfWork, statementRepository, transactionRepository, dateTimeService)
    where TTransaction : CreditCardTransaction
{
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    private protected static Transaction[] Except(IEnumerable<CreditCardTransaction> newOnes, IEnumerable<CreditCardTransaction> existing)
    {
        var groupedExisting = existing.GroupBy(x => new { x.Timestamp, x.Description, x.Amount, x.ProvisionState })
            .ToDictionary(x => x.Key, grp => grp.ToArray());

        var groupedNew = newOnes.GroupBy(x => new { x.Timestamp, x.Description, x.Amount, x.ProvisionState })
            .ToDictionary(x => x.Key, grp => grp.ToList());

        foreach (var key in groupedNew.Keys.Intersect(groupedExisting.Keys))
            groupedNew[key].RemoveRange(0, groupedExisting[key].Length);

        return groupedNew.SelectMany(x => x.Value).ToArray();
    }
}
