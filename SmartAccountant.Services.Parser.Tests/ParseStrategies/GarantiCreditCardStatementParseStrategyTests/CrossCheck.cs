using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;
using SmartAccountant.Shared.Structs;

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
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(30, Currency.USD),
                    Description = "",
                }
            },
        };

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.CrossCheck(statement));

        Assert.AreEqual(ParserErrors.TransactionAmountAndTotalExpensesMismatch, result.Error);
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
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(30, Currency.USD),
                    Description = "",
                },
                new CreditCardTransaction
                {
                    Amount = new MonetaryValue(-10, Currency.USD),
                    Description = "Debt Payment",
                }
            },
        };

        // Act
        sut.CrossCheck(statement);
    }
}
