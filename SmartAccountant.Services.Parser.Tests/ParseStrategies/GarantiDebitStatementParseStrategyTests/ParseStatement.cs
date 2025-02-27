using AutoFixture;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.GarantiDebitStatementParseStrategyTests;

[TestClass]
public class ParseStatement
{
    private GarantiDebitStatementParseStrategy sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ThrowArgumentExceptionForWrongStatement()
    {
        // Arrange
        var mockStatement = new Mock<Statement<DebitTransaction>>();

        // Act, Assert
        Assert.ThrowsExactly<ArgumentException>(() => sut.ParseStatement(mockStatement.Object, null!, null!));
    }

    [TestMethod]
    public void CreateNoTransactionForEmptyDocument()
    {
        // Arrange
        DebitStatement statement = new();

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
        DebitStatement statement = new();

        IEnumerable<Row> rows = GetHeaderRows();

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseStatement(statement, worksheet, null!);

        // Assert
        Assert.AreEqual(0, statement.Transactions.Count);
    }

    [TestMethod]
    public void ThrowParserExceptionWhenColumnCountLessThanFive()
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("100.00")),
            new Cell(new CellValue("900.00"))));

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("02/13/2025")]
    [DataRow("2025/01/13")]
    [DataRow("13-02-2025")]
    public void ThrowParserExceptionForMissingOrInvalidDate(string date)
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue(date)),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("100.00")),
            new Cell(new CellValue("900.00")),
            new Cell(new CellValue("Ref123"))));

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    [TestMethod]
    [DataRow("1", "a")]
    [DataRow("a", "1")]
    [DataRow("a", "a")]
    public void ThrowParserExceptionForInvalidAmountOrRemainingBalance(string amount, string remainingBalance)
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue(amount)),
            new Cell(new CellValue(remainingBalance)),
            new Cell(new CellValue("Ref123"))));

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    [TestMethod]
    public void ThrowParserExceptionForMissingSharedString()
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("100.00")),
            new Cell(new CellValue("900.00")),
            new Cell(new CellValue("0"))
            {
                DataType = CellValues.SharedString
            }));

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    [TestMethod]
    public void CreateOneTransaction()
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("100.00")),
            new Cell(new CellValue("900.00")),
            new Cell(new CellValue("Ref123"))));

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseStatement(statement, worksheet, null!);

        // Assert
        Assert.AreEqual(1, statement.Transactions.Count);

        DebitTransaction transaction = statement.Transactions.First();
        Assert.AreEqual(new DateTimeOffset(new DateTime(2025, 2, 13), TimeSpan.Zero), transaction.Timestamp);
        Assert.AreEqual(100.00m, transaction.Amount.Amount);
        Assert.AreEqual(900.00m, transaction.RemainingBalance.Amount);
        Assert.AreEqual("Ref123", transaction.ReferenceNumber);
        Assert.AreEqual("Description", transaction.Description);
    }

    [TestMethod]
    public void ThrowParserExceptionForInconsistentBalance()
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("100.00")),
            new Cell(new CellValue("900.00")),
            new Cell(new CellValue("Ref1"))));

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("-100.01")),
            new Cell(new CellValue("900.00")),
            new Cell(new CellValue("Ref2"))));

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, null!));
    }

    [TestMethod]
    public void SuccessfullyParseTransactions()
    {
        // Arrange
        DebitStatement statement = new();

        List<Row> rows = GetHeaderRows();

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("100.00")),
            new Cell(new CellValue("900.00")),
            new Cell(new CellValue("Ref1"))));

        rows.Add(new Row(
            new Cell(new CellValue("13/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("-10.00")),
            new Cell(new CellValue("890.00")),
            new Cell(new CellValue("Ref2"))));

        rows.Add(new Row(
            new Cell(new CellValue("14/02/2025")),
            new Cell(new CellValue("Description")),
            new Cell(new CellValue("-50.0001")),
            new Cell(new CellValue("839.9999")),
            new Cell(new CellValue("Ref3"))));

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseStatement(statement, worksheet, null!);

        // Assert
        Assert.AreEqual(3, statement.Transactions.Count);
        Assert.AreEqual(39.9999m, statement.Transactions.Sum(x => x.Amount.Amount));
    }

    private static List<Row> GetHeaderRows()
    {
        Fixture fixture = new();
        return fixture.CreateMany<Row>(GarantiDebitStatementParseStrategy.HeaderRowCount).ToList();
    }
}
