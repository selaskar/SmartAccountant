using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;

namespace SmartAccountant.Services;

internal class AccountService(IAccountRepository accountRepository, IAuthorizationService authorizationService) : IAccountService
{
    /// <inheritdoc/>
    public async Task<Account[]> GetAccountsOfUser(CancellationToken cancellationToken)
    {
        Guid userId = authorizationService.UserId;

        try
        {
            return await accountRepository.GetAccountsOfUser(userId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new AccountException(Messages.CannotFetchAccountsOfUser, ex);
        }
    }
}
