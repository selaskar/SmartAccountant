using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.AbstractGarantiCreditCardStatementParseStrategyTests;

[TestClass]
public class ParseSpan
{
    [TestMethod]
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        Row[] rows = [new Row()]; //insufficient column count

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() =>
            AbstractGarantiCreditCardStatementParseStrategy.ParseSpan(
                rows.AsSpan(), accountId: null, transactions: null!, ProvisionState.Finalized, stringTable: null!));
    }


    [TestMethod]
    public void NotThrowExceptionForEmptyRowList()
    {
        // Arrange
        Row[] rows = [];

        // Act
        AbstractGarantiCreditCardStatementParseStrategy.ParseSpan(
            rows.AsSpan(), accountId: null, transactions: null!, ProvisionState.Finalized, stringTable: null!);
    }

    [TestMethod]
    public void ParseValidRowSpan()
    {
        // Arrange
        Guid accountId = Guid.Parse("6E8E8CB9-9C28-401D-B7C0-CC4C13F42D54");

        List<CreditCardTransaction> transactions = [];
        SharedStringTable sharedStringTable = new();

        Row[] rows = [new Row(
            new Cell(new CellValue("01/09/2025")),
            new Cell(new CellValue("Expense 1")),
            new Cell(new CellValue("Label 1")),
            new Cell(new CellValue("0")),
            new Cell(new CellValue(sharedStringTable.GetNextReference()))
            {
                DataType = CellValues.SharedString
            })];

        sharedStringTable.AppendChild(new SharedStringItem(new Text("-130,00")));

        // Act
        AbstractGarantiCreditCardStatementParseStrategy.ParseSpan(rows.AsSpan(), accountId, transactions, ProvisionState.Finalized, sharedStringTable);

        // Assert
        Assert.AreEqual(1, transactions.Count);
        Assert.AreEqual(accountId, transactions[0].AccountId);
        Assert.AreEqual(ProvisionState.Finalized, transactions[0].ProvisionState);
        Assert.AreEqual(130m, transactions[0].Amount.Amount);
    }
}
