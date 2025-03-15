using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.CreditCardImportServiceTests;

[TestClass]
public class DetectFinalized : Base
{
    [TestMethod]
    public void ThrowImportExceptionForWrongStatement()
    {
        // Arrange
        DebitStatement statement = new();

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => sut.DetectFinalized(statement, null!));
    }

    [TestMethod]
    [DataRow(true, true, 0)]
    [DataRow(true, false, 0)]
    [DataRow(false, true, 1)]
    [DataRow(false, false, 0)]
    public void Succeed(bool open1, bool open2, int expected)
    {
        // Arrange
        var timestamp = new DateTimeOffset(new DateOnly(2025, 03, 14), TimeOnly.MinValue, TimeSpan.Zero);
        string description = "a";
        MonetaryValue amount = new(10, Currency.USD);

        CreditCardStatement statement = new()
        {
            Transactions = [ new CreditCardTransaction()
            {
                AccountId = Guid.Empty,
                Timestamp = timestamp,
                Description = description,
                Amount = amount,
                ProvisionState = open1 ? ProvisionState.Open: ProvisionState.Finalized
            }]
        };

        CreditCardTransaction[] existingTransactions = [ new CreditCardTransaction()
        {
            AccountId = Guid.Empty,
            Timestamp = timestamp,
            Description = description,
            Amount = amount,
            ProvisionState = open2 ? ProvisionState.Open: ProvisionState.Finalized
        }];

        // Act
        Transaction[] result = sut.DetectFinalized(statement, existingTransactions);

        // Assert
        Assert.AreEqual(expected, result.Length);
    }
}
