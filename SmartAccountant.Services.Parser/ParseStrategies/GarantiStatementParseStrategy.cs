//TODO: rename file to match class name.

using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Helpers;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed class GarantiDebitStatementParseStrategy : IStatementParseStrategy<DebitTransaction>
{
    /// <inheritdoc/>
    public void ParseStatement(Statement<DebitTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        DebitStatement debitStatement = statement as DebitStatement
            ?? throw new ArgumentException($"Statement expected to be type of {typeof(DebitStatement).Name}.");

        try
        {
            short rowCount = 0;
            foreach (Row row in worksheet.Descendants<Row>().Skip(11))
            {
                DebitTransaction transaction = ParseDebitTransaction(debitStatement, rowCount++, row, stringTable);
                statement.Transactions.Add(transaction);
            }
        }
        catch (Exception ex)
        {
            throw new ParserException("An error occurred while parsing the statement document.", ex);
        }

        CrossCheck(debitStatement);
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private static DebitTransaction ParseDebitTransaction(DebitStatement statement, short order, Row row, SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Amount (2), Remaining Balance (3), Reference Number (4)

        if (row.ChildElements.Count < 5)
            throw new ParserException($"Unrecognized file format. Column count ({row.ChildElements.Count}) was expected to be at least 5.");

        string dateString = row.GetCell(0).GetCellValue(stringTable);

        if (string.IsNullOrWhiteSpace(dateString))
            throw new ParserException($"Unrecognized file format. Expected to have transaction date at column A (item: {order + 1}).");

        if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
            throw new ParserException($"Unrecognized file format. Could not parse transaction date '{dateString}' (item: {order + 1}).");

        if (!row.GetCell(2).TryGetDecimalValue(out decimal? amountValue))
            throw new ParserException($"Transaction (item: {order + 1}) doesn't have a valid amount value.");

        if (!row.GetCell(3).TryGetDecimalValue(out decimal? remainingBalanceValue))
            throw new ParserException($"Transaction (item: {order + 1}) doesn't have a valid remaining amount value.");

        MonetaryValue amount = new(amountValue.Value.Round(), statement.Currency);
        MonetaryValue remainingBalance = new(remainingBalanceValue.Value.Round(), statement.Currency);

        return new DebitTransaction
        {
            Id = Guid.NewGuid(),
            StatementId = statement.Id,
            Timestamp = new DateTimeOffset(date, TimeSpan.Zero),
            Amount = amount,
            ReferenceNumber = row.GetCell(4).GetCellValue(stringTable),
            Note = row.GetCell(1).GetCellValue(stringTable),
            RemainingBalance = remainingBalance,
            Order = order
        };
    }

    /// <exception cref="ParserException"/>
    private static void CrossCheck(DebitStatement statement)
    {
        if (statement.Transactions.Count == 0)
            return;

        //TODO: test with 1 trx in total
        decimal totalAmount = statement.Transactions.Skip(1).Sum(x => x.Amount.Amount);

        decimal initialBalance = statement.Transactions.First().RemainingBalance.Amount;

        decimal closingBalance = statement.Transactions.Last().RemainingBalance.Amount;

        if (initialBalance + totalAmount != closingBalance)
            throw new ParserException("Total amount of transactions and remaining balance do not match.");
    }
}
