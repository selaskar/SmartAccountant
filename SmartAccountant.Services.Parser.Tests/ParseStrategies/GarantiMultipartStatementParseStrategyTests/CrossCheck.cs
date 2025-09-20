using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.GarantiMultipartStatementParseStrategyTests;

[TestClass]
public class CrossCheck
{
    private GarantiMultipartStatementParseStrategy sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();


    [TestMethod]
    public void NotThrowErrorForEmptyTransactionList()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalPayments = 0,
            TotalExpenses = 0,
        };

        // Act
        sut.CrossCheck(statement);
    }

    [TestMethod]
    public void ThrowErrorForInconsistentTotalExpenses()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 101,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                }
            },
        };

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));

        Assert.AreEqual(Messages.TransactionAmountAndTotalExpensesDontMatch, result.Message);
    }

    [TestMethod]
    public void ThrowErrorForHighDeflection()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 100 + GarantiMultipartStatementParseStrategy.DeflectionIgnoreThreshold + 1,
            TotalPayments = 50,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(100, Currency.USD),
                    Description = "işlem ücreti"
                },
                new CreditCardTransaction
                {
                    Description = "Debt Payment",
                    Amount = new MonetaryValue(-50, Currency.USD),
                }
            },
        };

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));
    }

    [TestMethod]
    public void NotThrowErrorForLowOrNoDeflection()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 100 + GarantiMultipartStatementParseStrategy.DeflectionIgnoreThreshold,
            TotalPayments = 50,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(100, Currency.USD),
                    Description = "işlem ücreti"
                },
                new CreditCardTransaction
                {
                    Description = "Debt Payment",
                    Amount = new MonetaryValue(-50, Currency.USD),
                }
            },
        };

        // Act, Assert
        sut.CrossCheck(statement);
    }
}
