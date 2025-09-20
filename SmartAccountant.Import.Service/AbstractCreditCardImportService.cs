using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service;

internal abstract class AbstractCreditCardImportService(
    ILogger<AbstractImportService> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IDateTimeService dateTimeService)
    : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService)
{
    protected internal static Transaction[] Except(IEnumerable<CreditCardTransaction> news, IEnumerable<CreditCardTransaction> existing)
    {
        var groupedExisting = existing.GroupBy(x => new { x.Timestamp, x.Description, x.Amount, x.ProvisionState })
            .ToDictionary(x => x.Key, grp => grp.ToArray());

        var groupedNew = news.GroupBy(x => new { x.Timestamp, x.Description, x.Amount, x.ProvisionState })
            .ToDictionary(x => x.Key, grp => grp.ToList());

        foreach (var key in groupedNew.Keys.Intersect(groupedExisting.Keys))
            groupedNew[key].RemoveRange(0, groupedExisting[key].Length);

        return groupedNew.SelectMany(x => x.Value).ToArray();
    }
}
