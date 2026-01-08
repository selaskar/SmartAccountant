using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.AbstractImportServiceTests;

[TestClass]
public class FetchExistingTransactions : Base
{
    [TestMethod]
    public async Task ThrowImportExceptionForErrorsInRepository()
    {
        // Arrange
        var accountId = Guid.Parse("F4A8598E-F6AB-4261-8E68-DE9552D412F4");

        DebitStatement statement = new()
        {
            AccountId = accountId,
        };

        SetupTransactionRepository(accountId).Throws(new ServerException("test", null!));

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.FetchExistingTransactions(statement, CancellationToken.None));
    }

    [TestMethod]
    public async Task ReturnTransactionsOfAccount()
    {
        // Arrange
        var accountId = Guid.Parse("473BB130-ADB6-4742-8FC1-337C2B840E34");

        DebitStatement statement = new()
        {
            AccountId = accountId,
        };

        SetupTransactionRepository(accountId).ReturnsAsync([new DebitTransaction()
        {
            AccountId = accountId,
            Description = "",
        }]);

        // Act
        Transaction[] result = await sut.FetchExistingTransactions(statement, CancellationToken.None);

        // Assert
        Assert.IsNotEmpty(result);
    }
}
