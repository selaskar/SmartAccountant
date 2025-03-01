using System.Globalization;
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
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        CreditCardStatement statement = new();

        Worksheet worksheet = new();

        SharedStringTable sharedStringTable = null!;

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, sharedStringTable));

        Assert.AreEqual(Messages.UnexpectedError, result.Message);
    }

    [TestMethod]
    public void CreateNoTransactionForEmptyDocument()
    {
        // Arrange
        CreditCardStatement statement = new();

        Worksheet worksheet = new();

        SharedStringTable sharedStringTable = new();

        // Act
        sut.ParseStatement(statement, worksheet, sharedStringTable);

        // Assert
        Assert.AreEqual(0, statement.Transactions.Count);
    }

    [TestMethod]
    public void SkipHeaderAndFooterRows()
    {
        // Arrange
        CreditCardStatement statement = new();

        List<Row> rows = GetHeaderRows();
        rows.AddRange(GetFooterRows());

        Worksheet worksheet = new(rows);

        SharedStringTable sharedStringTable = new();

        // Act
        sut.ParseStatement(statement, worksheet, sharedStringTable);

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

        SharedStringTable sharedStringTable = new();

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, sharedStringTable));
    }

    [TestMethod]
    public void ThrowParserExceptionForMissingSharedString()
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

        SharedStringTable sharedStringTable = new();

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, sharedStringTable));

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

        SharedStringTable sharedStringTable = new();

        // Act
        sut.ParseStatement(statement, worksheet, sharedStringTable);

        // Assert
        Assert.AreEqual(1, statement.Transactions.Count);

        CreditCardTransaction transaction = statement.Transactions.First();
        Assert.AreEqual(new DateTimeOffset(new DateTime(2025, 02, 27), TimeSpan.Zero), transaction.Timestamp);
        Assert.AreEqual(90.00m, transaction.Amount.Amount);
        Assert.AreEqual("Expense 1", transaction.Description);
    }

    [TestMethod]
    public void CombineOpenProvisionAndRegularTransaction()
    {
        // Arrange
        CreditCardStatement statement = new()
        {
            TotalDueAmount = 100
        };

        SharedStringTable sharedStringTable = new();

        List<Row> rows = GetOpenProvisionHeader(sharedStringTable);

        rows.Add(new Row(
            new Cell(new CellValue("27/02/2025")),
            new Cell(new CellValue("Open provision 1")),
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-10.00"))));

        rows.AddRange(GetOpenProvisionFooter(sharedStringTable));

        rows.AddRange(GetHeaderRows().Skip(1)); // Skipping icon row, as it doesn't repeat after first section.

        rows.Add(new Row(
            new Cell(new CellValue("27/02/2025")),
            new Cell(new CellValue("Expense 1")),
            new Cell(new CellValue("Label 2")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue("-90.00"))));

        rows.AddRange(GetFooterRows());

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseStatement(statement, worksheet, sharedStringTable);

        // Assert
        Assert.AreEqual(2, statement.Transactions.Count);
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

        SharedStringTable sharedStringTable = new();

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseStatement(statement, worksheet, sharedStringTable));
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

    private static List<Row> GetOpenProvisionHeader(SharedStringTable stringTable)
    {
        //Assuming 3 rows for open provision header
        Row rowIcon = new();

        //Setting a string reference based on the total number of elements already in the table.
        string stringReference = stringTable.ChildElements.Count.ToString(CultureInfo.InvariantCulture);
        Row rowOpenProvisionLabel = new(new Cell(new CellValue(stringReference))
        {
            DataType = CellValues.SharedString
        });
        stringTable.AppendChild(new SharedStringItem(new Text(GarantiCreditCardStatementParseStrategy.OpenProvisionLabel)));


        stringReference = stringTable.ChildElements.Count.ToString(CultureInfo.InvariantCulture);
        Row rowRegularLabel = new(new Cell(new CellValue(stringReference))
        {
            DataType = CellValues.SharedString
        });
        stringTable.AppendChild(new SharedStringItem(new Text(GarantiCreditCardStatementParseStrategy.RegularTransactionsLabel)));

        return [rowIcon, rowOpenProvisionLabel, rowRegularLabel];
    }

    private static List<Row> GetOpenProvisionFooter(SharedStringTable stringTable)
    {
        // Assuming 1 row for open provision footer

        string stringReference = stringTable.ChildElements.Count.ToString(CultureInfo.InvariantCulture);
        Row rowTotalAmount = new(new Cell(new CellValue(stringReference))
        {
            DataType = CellValues.SharedString
        });

        stringTable.AppendChild(new SharedStringItem(new Text(GarantiCreditCardStatementParseStrategy.OpenProvisionTotalAmountLabel)));

        return [rowTotalAmount];
    }
}
