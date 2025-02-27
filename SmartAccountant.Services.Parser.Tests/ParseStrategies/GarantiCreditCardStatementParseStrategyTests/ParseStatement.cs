using AutoFixture;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.GarantiCreditCardStatementParseStrategyTests;

[TestClass]
public class ParseStatement
{
    private GarantiCreditCardStatementParseStrategy sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ThrowArgumentExceptionForWrongStatement()
    {
        // Arrange
        var mockStatement = new Mock<Statement<CreditCardTransaction>>();

        // Act, Assert
        Assert.ThrowsExactly<ArgumentException>(() => sut.ParseStatement(mockStatement.Object, null!, null!));
    }

    [TestMethod]
    public void CreateNoTransactionForEmptyDocument()
    {
        // Arrange
        CreditCardStatement statement = new();

        Worksheet worksheet = new();

        // Act
        sut.ParseStatement(statement, worksheet, null!);

        // Assert
        Assert.AreEqual(0, statement.Transactions.Count);
    }

    [TestMethod]
    public void SkipHeaderRows()
    {
        // Arrange
        CreditCardStatement statement = new();

        IEnumerable<Row> rows = GetHeaderRows();

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseStatement(statement, worksheet, null!);

        // Assert
        Assert.AreEqual(0, statement.Transactions.Count);
    }

    [TestMethod]
    public void ThrowParserExceptionForInvalidAmount()
    {
        // Arrange
        CreditCardStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("27/02/2025")),
            new Cell(new CellValue("Expense 1")),
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-a.00"))));

        rows.AddRange(GetFooterRows());

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    [TestMethod]
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        CreditCardStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("27/02/2025")),
            new Cell(new CellValue("1"))
            {
                DataType = CellValues.SharedString
            },
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-90.00"))));

        rows.AddRange(GetFooterRows());

        Worksheet worksheet = new(rows);

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));

        Assert.AreEqual(Messages.UnexpectedError, result.Message);
    }

    [TestMethod]
    public void CreateOneTransaction()
    {
        // Arrange
        CreditCardStatement statement = new()
        {
            TotalDueAmount = 90
        };

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("27/02/2025")),
            new Cell(new CellValue("Expense 1")),
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-90.00"))));

        rows.AddRange(GetFooterRows());

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseStatement(statement, worksheet, null!);

        // Assert
        Assert.AreEqual(1, statement.Transactions.Count);

        CreditCardTransaction transaction = statement.Transactions.First();
        Assert.AreEqual(new DateTimeOffset(new DateTime(2025, 02, 27), TimeSpan.Zero), transaction.Timestamp);
        Assert.AreEqual(90.00m, transaction.Amount.Amount);
        Assert.AreEqual("Expense 1", transaction.Description);
    }

    [TestMethod]
    public void ThrowParserExceptionForInconsistentBalance()
    {
        // Arrange
        CreditCardStatement statement = new()
        {
            TotalDueAmount = 130
        };

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("27/02/2025")),
            new Cell(new CellValue("Expense 1")),
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-90.00"))));

        rows.Add(new Row(
            new Cell(new CellValue("28/02/2025")),
            new Cell(new CellValue("Expense 2")),
            new Cell(new CellValue("Label 2")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-40.01"))));

        rows.AddRange(GetFooterRows());

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    private static List<Row> GetHeaderRows()
    {
        Fixture fixture = new();
        return fixture.CreateMany<Row>(GarantiCreditCardStatementParseStrategy.HeaderRowCount).ToList();
    }

    private static List<Row> GetFooterRows()
    {
        Fixture fixture = new();
        return fixture.CreateMany<Row>(GarantiCreditCardStatementParseStrategy.FooterRowCount).ToList();
    }
}
