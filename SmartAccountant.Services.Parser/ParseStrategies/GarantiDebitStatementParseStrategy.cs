using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed class GarantiDebitStatementParseStrategy : AbstractGarantiStatementParseStrategy, IStatementParseStrategy<DebitTransaction>
{
    /// Non-empty rows
    internal const int HeaderRowCount = 11;

    private static readonly CompositeFormat UnexpectedRemainingAmountFormat = CompositeFormat.Parse(Messages.UnexpectedRemainingAmountFormat);

    /// <inheritdoc/>
    public void ParseStatement(Statement<DebitTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        var debitStatement = statement as DebitStatement
            ?? throw new ArgumentException($"Statement expected to be type of {typeof(DebitStatement).Name}.");

        try
        {
            short rowCount = 0;
            foreach (Row row in worksheet.Descendants<Row>().Skip(HeaderRowCount))
            {
                DebitTransaction transaction = ParseDebitTransaction(debitStatement, rowCount++, row, stringTable);
                statement.Transactions.Add(transaction);
            }
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedError, ex);
        }

        CrossCheck(debitStatement);
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private static DebitTransaction ParseDebitTransaction(DebitStatement statement, short order, Row row, SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Amount (2), Remaining Balance (3), Reference Number (4)

        VerifyColumnCount(row, 5);

        DateTimeOffset date = ParseDate(row, column: 0, stringTable, order);

        if (!ParseMoney(row, column: 2, statement.Currency, out MonetaryValue? amount))
            throw new ParserException(FormatMessage(UnexpectedAmountFormat, order + 1));

        if (!ParseMoney(row, column: 3, statement.Currency, out MonetaryValue? remainingBalance))
            throw new ParserException(FormatMessage(UnexpectedRemainingAmountFormat, order + 1));

        return new DebitTransaction
        {
            Id = Guid.NewGuid(),
            StatementId = statement.Id,
            Timestamp = date,
            Amount = amount.Value,
            ReferenceNumber = row.GetCell(4).GetCellValue(stringTable),
            Description = row.GetCell(1).GetCellValue(stringTable),
            RemainingBalance = remainingBalance.Value,
            Order = order
        };
    }


    /// <exception cref="ParserException"/>
    private static void CrossCheck(DebitStatement statement)
    {
        if (statement.Transactions.Count == 0)
            return;

        decimal totalAmount = statement.Transactions.Skip(1).Sum(x => x.Amount.Amount);

        decimal initialBalance = statement.Transactions.First().RemainingBalance.Amount;

        decimal closingBalance = statement.Transactions.Last().RemainingBalance.Amount;

        if (initialBalance + totalAmount != closingBalance)
            throw new ParserException(Messages.TransactionAmountAndBalanceNotMatch);
    }
}
