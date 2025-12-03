using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;

namespace SmartAccountant.Services.Tests.AccountServiceTests;

[TestClass]
public class GetAccountsOfUser
{
    private Mock<IAccountRepository> accountRepositoryMock = null!;
    private Mock<IAuthorizationService> authorizationServiceMock = null!;

    private AccountService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        accountRepositoryMock = new();
        authorizationServiceMock = new();

        sut = new AccountService(accountRepositoryMock.Object, authorizationServiceMock.Object);
    }

    [TestMethod]
    public async Task ThrowAuthenticationExceptionForUnauthenticatedUser()
    {
        // Arrange
        authorizationServiceMock.SetupGet(a => a.UserId).Throws(new AuthenticationException("test"));

        // Act, Assert
        await Assert.ThrowsExactlyAsync<AuthenticationException>(async () => await sut.GetAccountsOfUser(CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowAccountExceptionForUnexpectedErrorInAccountRepository()
    {
        // Arrange
        authorizationServiceMock.SetupGet(a => a.UserId).Returns(Guid.Empty);

        accountRepositoryMock.Setup(x => x.GetAccountsOfUser(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<AccountException>(async () => await sut.GetAccountsOfUser(CancellationToken.None));

        Assert.AreEqual(Messages.CannotFetchAccountsOfUser, result.Message);
    }

    [TestMethod]
    public async Task SuccessfullyReturnAccounts()
    {
        // Arrange
        authorizationServiceMock.SetupGet(a => a.UserId).Returns(Guid.Empty);

        accountRepositoryMock.Setup(x => x.GetAccountsOfUser(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Array.Empty<Account>()));

        // Act
        Account[] result = await sut.GetAccountsOfUser(CancellationToken.None);

        // Assert
        Assert.AreEqual(0, result.Length);
    }
}
