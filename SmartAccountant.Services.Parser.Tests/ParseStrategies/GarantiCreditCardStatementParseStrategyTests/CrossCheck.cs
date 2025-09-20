using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.GarantiCreditCardStatementParseStrategyTests;

[TestClass]
public class CrossCheck
{
    private GarantiCreditCardStatementParseStrategy sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();


    [TestMethod]
    public void NotThrowErrorForEmptyTransactionList()
    {
        // Arrange
        CreditCardStatement statement = new();

        // Act
        sut.CrossCheck(statement);
    }

    [TestMethod]
    public void ThrowErrorForInconsistentTotalAmount()
    {
        // Arrange
        CreditCardStatement statement = new()
        {
            TotalExpenses = 81,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(50, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(30, Currency.USD),
                }
            },
        };

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));

        Assert.AreEqual(Messages.TransactionAmountAndTotalExpensesDontMatch, result.Message);
    }

    [TestMethod]
    public void NotThrowErrorForConsistentTotalAmount()
    {
        // Arrange
        CreditCardStatement statement = new()
        {
            TotalExpenses = 80,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(50, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(30, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Description = "Debt Payment",
                    Amount = new MonetaryValue(-10, Currency.USD),
                }
            },
        };

        // Act
        sut.CrossCheck(statement);
    }
}
