using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Import.Service.Tests.DebitImportServiceTests;

[TestClass]
public class DetectNew : Base
{
    [TestMethod]
    public void ThrowImportExceptionForWrongStatement()
    {
        // Arrange
        CreditCardStatement creditCardStatement = new();

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => sut.DetectNew(creditCardStatement, null!));
    }

    [TestMethod]
    [DataRow("a", "a", 10, 10, Currency.USD, Currency.USD, 0)]
    [DataRow("a", "a", 10, 10, Currency.USD, Currency.EUR, 1)]
    [DataRow("a", "a", 10, 09, Currency.USD, Currency.USD, 1)]
    [DataRow("a", "a", 10, 09, Currency.USD, Currency.EUR, 1)]
    [DataRow("a", "A", 10, 10, Currency.USD, Currency.USD, 1)]
    [DataRow("a", "A", 10, 10, Currency.USD, Currency.EUR, 1)]
    [DataRow("a", "A", 10, 09, Currency.USD, Currency.USD, 1)]
    [DataRow("a", "A", 10, 09, Currency.USD, Currency.EUR, 1)]
    public void Succeed(string refNumber1, string refNumber2, int remaining1, int remaining2, Currency currency1, Currency currency2, int expected)
    {
        // Arrange
        DebitStatement debitStatement = new()
        {
            Transactions = [ new DebitTransaction()
            {
                AccountId = Guid.Empty,
                ReferenceNumber = refNumber1,
                Description = "",
                RemainingBalance = new MonetaryValue(remaining1, currency1),
            }]
        };

        DebitTransaction[] existingTransactions = [ new DebitTransaction()
        {
            AccountId = Guid.Empty,
            ReferenceNumber = refNumber2,
            Description = "",
            RemainingBalance = new MonetaryValue(remaining2, currency2),
        }];

        // Act
        Transaction[] result = sut.DetectNew(debitStatement, existingTransactions);

        // Assert
        Assert.HasCount(expected, result);
    }
}
