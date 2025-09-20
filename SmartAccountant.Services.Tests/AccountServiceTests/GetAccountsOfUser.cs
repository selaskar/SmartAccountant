using System.Linq;
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
    public void ThrowAuthenticationExceptionForUnauthenticatedUser()
    {
        // Arrange
        authorizationServiceMock.SetupGet(a => a.UserId).Throws(new AuthenticationException("test"));

        // Act, Assert
        Assert.ThrowsExactly<AuthenticationException>(() => sut.GetAccountsOfUser());
    }

    [TestMethod]
    public void ThrowAccountExceptionForUnexpectedErrorInAccountRepository()
    {
        // Arrange
        authorizationServiceMock.SetupGet(a => a.UserId).Returns(Guid.Empty);

        accountRepositoryMock.Setup(x => x.GetAccountsOfUser(It.IsAny<Guid>())).Throws<InvalidOperationException>();

        // Act, Assert
        var result = Assert.ThrowsExactly<AccountException>(() => sut.GetAccountsOfUser());

        Assert.AreEqual(Messages.CannotFetchAccountsOfUser, result.Message);
    }

    [TestMethod]
    public async Task SuccessfullyReturnAccounts()
    {
        // Arrange
        authorizationServiceMock.SetupGet(a => a.UserId).Returns(Guid.Empty);
        
        accountRepositoryMock.Setup(x => x.GetAccountsOfUser(It.IsAny<Guid>())).Returns(AsyncEnumerable.Empty<Account>());

        // Act
        IAsyncEnumerable<Account> result = sut.GetAccountsOfUser();

        // Assert
        Assert.AreEqual(0, await result.CountAsync());
    }
}
