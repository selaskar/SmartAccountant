using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.GarantiMultipartStatementParseStrategyTests;

[TestClass]
public class ParseMultipartStatement
{
    private GarantiMultipartStatementParseStrategy sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ThrowParserExceptionForInsufficientSectionCount()
    {
        // Arrange
        SharedStatement statement = new();

        List<Row> rows = GetSectionHeader("1234 **** **** 5678");

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseMultipartStatement(statement, worksheet, null!));
    }

    [TestMethod]
    public void ThrowParserExceptionForExcessiveSectionCount()
    {
        // Arrange
        SharedStatement statement = new();

        List<Row> rows = GetSectionHeader("1234 **** **** 5678")
            .Union(GetSectionHeader("2345 **** **** 5678"))
            .Union(GetSectionHeader("3456 **** **** 5678"))
            .ToList();

        Worksheet worksheet = new(rows);

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ParseMultipartStatement(statement, worksheet, null!));
    }

    [TestMethod]
    public void ParseOneTransactionForEachSection()
    {
        // Arrange
        SharedStatement statement = new();

        List<Row> rows = GetSectionHeader("1234 **** **** 5678");

        SharedStringTable sharedStringTable = new();

        rows.Add(new Row(
            new Cell(new CellValue("01/09/2025")),
            new Cell(new CellValue("Expense 1")),
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue(sharedStringTable.GetNextReference()))
            {
                DataType = CellValues.SharedString
            }));

        sharedStringTable.AppendChild(new SharedStringItem(new Text("-130,00")));

        rows.AddRange(GetSectionHeader("2345 **** **** 5678"));

        rows.Add(new Row(
            new Cell(new CellValue("31/08/2025")),
            new Cell(new CellValue("Expense 2")),
            new Cell(new CellValue("Label 2")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue(sharedStringTable.GetNextReference()))
            {
                DataType = CellValues.SharedString
            }));

        sharedStringTable.AppendChild(new SharedStringItem(new Text("-115,00")));

        Worksheet worksheet = new(rows);

        // Act
        sut.ParseMultipartStatement(statement, worksheet, sharedStringTable);

        // Assert
        Assert.AreEqual(1, statement.Transactions.Count);
        Assert.AreEqual(1, statement.SecondaryTransactions.Count);

        Assert.AreEqual(ProvisionState.Finalized, statement.Transactions[0].ProvisionState);
        Assert.AreEqual(ProvisionState.Finalized, statement.SecondaryTransactions[0].ProvisionState);

        Assert.AreEqual("1234 **** **** 5678", statement.CardNumber1);
        Assert.AreEqual("2345 **** **** 5678", statement.CardNumber2);
    }


    private static List<Row> GetSectionHeader(string cardNumber)
    {
        Row cardNumberRow = new(
            new Cell(new CellValue($"{cardNumber} Numarali Kart TL Ekstre Bilgileri")));

        Row tableHeader = new();

        return [cardNumberRow, tableHeader];
    }
}
