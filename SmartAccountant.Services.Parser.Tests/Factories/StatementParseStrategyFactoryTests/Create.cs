﻿using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Factories;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.Factories.StatementParseStrategyFactoryTests;

[TestClass]
public class Create
{
    private StatementParseStrategyFactory sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ThrowNotImplementedExceptionForUnsupportedTransactionType()
    {
        // Act, Assert
        Assert.ThrowsException<NotImplementedException>(() => sut.Create<Transaction>(Bank.GarantiBBVA));
    }

    [TestMethod]
    public void ThrowNotImplementedExceptionForUnsupportedBank()
    {
        // Act, Assert
        Assert.ThrowsException<NotImplementedException>(() => sut.Create<DebitTransaction>(Bank.Unknown));
    }

    [TestMethod]
    public void CreateParseStrategyForGarantiDebitTransaction()
    {
        // Act
        IStatementParseStrategy<DebitTransaction>? parseStrategy = sut.Create<DebitTransaction>(Bank.GarantiBBVA);

        // Assert
        Assert.IsNotNull(parseStrategy);
        Assert.IsInstanceOfType<GarantiDebitStatementParseStrategy>(parseStrategy);
    }
}
