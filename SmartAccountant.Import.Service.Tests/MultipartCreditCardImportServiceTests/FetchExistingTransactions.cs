using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.MultipartCreditCardImportServiceTests;

[TestClass]
public class FetchExistingTransactions : Base
{
    [TestMethod]
    public async Task ThrowImportExceptionForWrongStatement()
    {
        // Arrange
        DebitStatement statement = new();

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.FetchExistingTransactions(statement, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementFactory()
    {
        // Arrange
        var accountId = Guid.Parse("B7A0BFC2-BE30-4483-A13E-0A6B6A07AFDC");

        SharedStatement statement = new()
        {
            AccountId = accountId,
        };

        SetupTransactionRepository(accountId).Throws(new RepositoryException("test", null!));

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.FetchExistingTransactions(statement, CancellationToken.None));
    }

    [TestMethod]
    public async Task ReturnTransactionsOfBothAccounts()
    {
        // Arrange
        var accountId = Guid.Parse("3907FF3B-4A7D-4779-8088-7BAED932E585");
        var dependentAccountId = Guid.Parse("3ACBED90-9DFC-4E84-9558-F0B30EA7A509");

        SharedStatement statement = new()
        {
            AccountId = accountId,
            DependentAccountId = dependentAccountId,
        };

        SetupTransactionRepository(accountId).ReturnsAsync([new CreditCardTransaction()
        {
            AccountId = accountId,
        }]);

        SetupTransactionRepository(dependentAccountId).ReturnsAsync([new CreditCardTransaction()
        {
            AccountId = dependentAccountId,
        }]);

        // Act
        Transaction[] result = await sut.FetchExistingTransactions(statement, CancellationToken.None);

        // Assert
        Assert.AreEqual(2, result.Length);
    }
}
