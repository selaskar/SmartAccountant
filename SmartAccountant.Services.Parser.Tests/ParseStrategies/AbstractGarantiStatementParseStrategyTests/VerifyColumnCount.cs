using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.AbstractGarantiStatementParseStrategyTests;

[TestClass]
public class VerifyColumnCount
{
    [TestMethod]
    public void ThrowParserExceptionForInsufficientColumnCount()
    {
        // Arrange
        Row row = new Row(new Cell(), new Cell());

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => AbstractGarantiStatementParseStrategy.VerifyColumnCount(row, 3));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void NotRaiseErrorForSufficientColumnCount(int expectedColumnCount)
    {
        // Arrange
        Row row = new Row(new Cell(), new Cell());

        // Act
        AbstractGarantiStatementParseStrategy.VerifyColumnCount(row, expectedColumnCount);
    }
}
