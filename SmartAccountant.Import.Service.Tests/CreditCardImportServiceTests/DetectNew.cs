using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Import.Service.Tests.CreditCardImportServiceTests;

[TestClass]
public class DetectNew : Base
{
    [TestMethod]
    public void ThrowImportExceptionForWrongStatement()
    {
        // Arrange
        DebitStatement statement = new();

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => sut.DetectNew(statement, null!));
    }

    [TestMethod]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 0)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    public void Succeed(
        string date1, string date2,
        string description1, string description2,
        int amount1, int amount2,
        Currency currency1, Currency currency2,
        ProvisionState state1, ProvisionState state2,
        int expected)
    {
        // Arrange
        CreditCardStatement statement = new()
        {
            Transactions = [ new CreditCardTransaction()
            {
                AccountId = Guid.Empty,
                Timestamp = new DateTimeOffset(DateOnly.ParseExact(date1, "yyyy-MM-dd"), TimeOnly.MinValue,TimeSpan.Zero),
                Description = description1,
                Amount = new MonetaryValue(amount1, currency1),
                ProvisionState = state1
            }]
        };

        CreditCardTransaction[] existingTransactions = [ new CreditCardTransaction()
        {
            AccountId = Guid.Empty,
            Timestamp = new DateTimeOffset(DateOnly.ParseExact(date2, "yyyy-MM-dd"), TimeOnly.MinValue,TimeSpan.Zero),
            Description = description2,
            Amount = new MonetaryValue(amount2, currency2),
            ProvisionState = state2
        }];

        // Act
        Transaction[] result = sut.DetectNew(statement, existingTransactions);

        // Assert
        Assert.AreEqual(expected, result.Length);
    }
}
