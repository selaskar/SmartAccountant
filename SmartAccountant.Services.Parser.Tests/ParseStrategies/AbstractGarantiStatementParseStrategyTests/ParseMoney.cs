using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.AbstractGarantiStatementParseStrategyTests;

[TestClass]
public class ParseMoney
{
    [TestMethod]
    public void ThrowArgumentOutOfRangeExceptionForInvalidColumnNumber()
    {
        // Arrange
        Cell cell = new(new CellValue(3.5))
        {
            DataType = CellValues.Number
        };

        Row row = new(cell);

        // Act, Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            AbstractGarantiStatementParseStrategy.ParseMoney(row, column: 1, Currency.USD, out _));
    }

    [TestMethod]
    public void ReturnNullForNonNumericValue()
    {
        // Arrange
        Cell cell = new(new CellValue("a"))
        {
            DataType = CellValues.InlineString
        };

        Row row = new(cell);

        // Act
        bool result = AbstractGarantiStatementParseStrategy.ParseMoney(row, column: 0, Currency.USD, out MonetaryValue? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void ParseNumericValue()
    {
        // Arrange
        Cell cell = new(new CellValue(3.5))
        {
            DataType = CellValues.Number
        };

        Row row = new(cell);

        // Act
        bool result = AbstractGarantiStatementParseStrategy.ParseMoney(row, column: 0, Currency.USD, out MonetaryValue? value);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(value);
        Assert.AreEqual(3.5m, value.Value.Amount);
        Assert.AreEqual(Currency.USD, value.Value.Currency);
    }


    [TestMethod]
    public void ReThrowArgumentOutOfRangeException()
    {
        // Arrange
        Cell cell = new(new CellValue(3.5))
        {
            DataType = CellValues.Number
        };

        Row row = new(cell);

        // Act, Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            AbstractGarantiStatementParseStrategy.ParseMoney(row, column: 1, Currency.USD, defaultIfEmpty: 3.5m, out _));
    }

    [TestMethod]
    public void ReReturnNullForNonNumericValue()
    {
        // Arrange
        Cell cell = new(new CellValue("a"))
        {
            DataType = CellValues.InlineString
        };

        Row row = new(cell);

        // Act
        bool result = AbstractGarantiStatementParseStrategy.ParseMoney(row, column: 0, Currency.USD, defaultIfEmpty: 3.5m, out MonetaryValue? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void ReturnDefaultValueForMissingText(string text)
    {
        // Arrange
        Cell cell = new(new CellValue(text))
        {
            DataType = CellValues.InlineString
        };

        Row row = new(cell);

        // Act
        bool result = AbstractGarantiStatementParseStrategy.ParseMoney(row, column: 0, Currency.USD, defaultIfEmpty: 3.5m, out MonetaryValue? value);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(value);
        Assert.AreEqual(3.5m, value.Value.Amount);
        Assert.AreEqual(Currency.USD, value.Value.Currency);
    }
}
