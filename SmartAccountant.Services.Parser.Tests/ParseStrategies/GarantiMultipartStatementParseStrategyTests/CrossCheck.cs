using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

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
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                    Description = "",
                }
            },
        };

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));

        Assert.AreEqual(Messages.TransactionAmountAndTotalExpensesDontMatch, result.Message);
    }

    [TestMethod]
    public void ThrowErrorForPositiveBigDeflection()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 120 + GarantiMultipartStatementParseStrategy.DeflectionIgnoreThreshold - 1,
            TotalPayments = 50,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(100, Currency.USD),
                    Description = "fee"
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
    public void ThrowErrorForNegativeBigDeflection()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 50 + GarantiMultipartStatementParseStrategy.DeflectionIgnoreThreshold + 1,
            TotalPayments = 50,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(100, Currency.USD),
                    Description = "fee"
                },
                new CreditCardTransaction
                {
                    Description = "Debt Payment",
                    Amount = new MonetaryValue(-200, Currency.USD),
                }
            },
        };

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));
    }

    [TestMethod]
    public void NotThrowErrorForPositiveSmallOrNoDeflection()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 120 + GarantiMultipartStatementParseStrategy.DeflectionIgnoreThreshold,
            TotalPayments = 50,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(100, Currency.USD),
                    Description = "fee"
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

    [TestMethod]
    public void NotThrowErrorForNegativeSmallOrNoDeflection()
    {
        // Arrange
        SharedStatement statement = new()
        {
            TotalExpenses = 50 + GarantiMultipartStatementParseStrategy.DeflectionIgnoreThreshold,
            TotalPayments = 50,
            Transactions =
            {
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(80, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(20, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(100, Currency.USD),
                    Description = "fee"
                },
                new CreditCardTransaction
                {
                    Description = "Debt Payment",
                    Amount = new MonetaryValue(-200, Currency.USD),
                }
            },
        };

        // Act, Assert
        sut.CrossCheck(statement);
    }
}
