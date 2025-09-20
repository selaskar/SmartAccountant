using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.AbstractCreditCardImportServiceTests;

[TestClass]
public class Except
{
    [TestMethod]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 0)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "a", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-14", "a", "A", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "a", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "a", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "a", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "a", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "a", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "a", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 10, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 10, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 09, Currency.USD, Currency.USD, ProvisionState.Finalized, ProvisionState.Open, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Finalized, 1)]
    [DataRow("2025-03-14", "2025-03-15", "a", "A", 10, 09, Currency.USD, Currency.EUR, ProvisionState.Finalized, ProvisionState.Open, 1)]
    public void Succeed(
        string date1, string date2,
        string description1, string description2,
        int amount1, int amount2,
        Currency currency1, Currency currency2,
        ProvisionState state1, ProvisionState state2,
        int expected)
    {
        // Arrange
        CreditCardTransaction[] newTransactions = [ new ()
        {
            AccountId = Guid.Empty,
            Timestamp = new DateTimeOffset(DateOnly.ParseExact(date1, "yyyy-MM-dd"), TimeOnly.MinValue,TimeSpan.Zero),
            Description = description1,
            Amount = new MonetaryValue(amount1, currency1),
            ProvisionState = state1
        }];

        CreditCardTransaction[] existingTransactions = [ new CreditCardTransaction()
        {
            AccountId = Guid.Empty,
            Timestamp = new DateTimeOffset(DateOnly.ParseExact(date2, "yyyy-MM-dd"), TimeOnly.MinValue,TimeSpan.Zero),
            Description = description2,
            Amount = new MonetaryValue(amount2, currency2),
            ProvisionState = state2
        }];

        // Act
        Transaction[] result = AbstractCreditCardImportService.Except(newTransactions, existingTransactions);

        // Assert
        Assert.AreEqual(expected, result.Length);
    }
}
