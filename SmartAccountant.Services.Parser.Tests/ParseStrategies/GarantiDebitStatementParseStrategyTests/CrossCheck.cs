using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.GarantiDebitStatementParseStrategyTests;

[TestClass]
public class CrossCheck
{
    private GarantiDebitStatementParseStrategy sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();


    [TestMethod]
    public void NotThrowErrorForEmptyTransactionList()
    {
        // Arrange
        DebitStatement statement = new();

        // Act
        sut.CrossCheck(statement);
    }

    [TestMethod]
    public void NotThrowErrorForOneTransaction()
    {
        // Arrange
        DebitStatement statement = new()
        {
            Transactions =
            {
                new DebitTransaction
                {
                    Amount = new MonetaryValue(-50, Currency.USD),
                    Description = "",
                    RemainingBalance = new MonetaryValue(100, Currency.USD)
                }
            }
        };

        // Act
        sut.CrossCheck(statement);
    }

    [TestMethod]
    public void ThrowErrorForInconsistentBalance()
    {
        // Arrange
        DebitStatement statement = new()
        {
            Transactions =
            {
                new DebitTransaction
                {
                    Amount = new MonetaryValue(-50, Currency.USD),
                    Description = "",
                    RemainingBalance = new MonetaryValue(100, Currency.USD)
                },
                new DebitTransaction
                {
                    Amount = new MonetaryValue(10, Currency.USD),
                    Description = "",
                    RemainingBalance = new MonetaryValue(111, Currency.USD)
                }
            }
        };

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));

        Assert.AreEqual(ParserErrors.TransactionAmountAndBalanceMismatch, result.Error);
    }

    [TestMethod]
    public void NotThrowErrorForConsistentBalance()
    {
        // Arrange
        DebitStatement statement = new()
        {
            Transactions =
            {
                new DebitTransaction
                {
                    Amount = new MonetaryValue(-50, Currency.USD),
                    Description = "",
                    RemainingBalance = new MonetaryValue(100, Currency.USD)
                },
                new DebitTransaction
                {
                    Amount = new MonetaryValue(10, Currency.USD),
                    Description = "",
                    RemainingBalance = new MonetaryValue(110, Currency.USD)
                }
            }
        };

        // Act
        sut.CrossCheck(statement);
    }
}
