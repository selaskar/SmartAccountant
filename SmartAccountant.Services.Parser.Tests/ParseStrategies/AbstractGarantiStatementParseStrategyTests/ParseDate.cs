using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.AbstractGarantiStatementParseStrategyTests;

[TestClass]
public class ParseDate
{
    [TestMethod]
    public void ThrowArgumentNullExceptionForMissingStringTable()
    {
        // Arrange
        Cell dateCell = new(new CellValue("0"))
        {
            DataType = CellValues.SharedString
        };

        Row row = new(dateCell);

        // Act, Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AbstractGarantiStatementParseStrategy.ParseDate(row, column: 0, stringTable: null!, order: 0));
    }

    [TestMethod]
    public void ThrowArgumentOutOfRangeExceptionForInvalidColumnNumber()
    {
        // Arrange
        Cell dateCell = new(new CellValue("0"))
        {
            DataType = CellValues.SharedString
        };

        SharedStringTable stringTable = new(new SharedStringItem(new Text("13/02/2025")));

        Row row = new(dateCell);

        // Act, Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            AbstractGarantiStatementParseStrategy.ParseDate(row, column: 1, stringTable, order: 0));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("02/13/2025")]
    [DataRow("02-13-2025")]
    [DataRow("02.13.2025")]
    [DataRow("2025/02/13")]
    [DataRow("2025-02-13")]
    [DataRow("2025.02.13")]
    [DataRow("13022025")]
    [DataRow("20250213")]
    public void ThrowParserExceptionForMissingDateOrInvalidFormat(string dateString)
    {
        // Arrange
        Cell dateCell = new(new CellValue("0"))
        {
            DataType = CellValues.SharedString
        };

        SharedStringTable stringTable = new(new SharedStringItem(new Text(dateString)));

        Row row = new(dateCell);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() =>
            AbstractGarantiStatementParseStrategy.ParseDate(row, column: 0, stringTable, order: 0));
    }

    [TestMethod]
    public void ParseDateInStringTable()
    {
        // Arrange
        Cell dateCell = new(new CellValue("0"))
        {
            DataType = CellValues.SharedString
        };

        SharedStringTable stringTable = new(new SharedStringItem(new Text("13/02/2025")));

        Row row = new(dateCell);

        // Act
        DateTimeOffset result = AbstractGarantiStatementParseStrategy.ParseDate(row, column: 0, stringTable, order: 0);

        // Assert
        Assert.AreEqual(new DateTimeOffset(2025, 02, 13, 0, 0, 0, TimeSpan.Zero), result);
    }
}
