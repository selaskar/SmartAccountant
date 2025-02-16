using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Helpers;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed class GarantiDebitStatementParseStrategy : IStatementParseStrategy<DebitTransaction>
{
    /// Non-empty rows
    internal const int HeaderRowCount = 11;

    private static readonly CompositeFormat InsufficientRowNumber = CompositeFormat.Parse(Messages.InsufficientRowNumber);
    private static readonly CompositeFormat TransactionDateColumnMissing = CompositeFormat.Parse(Messages.TransactionDateColumnMissing);
    private static readonly CompositeFormat UnexpectedDateFormat = CompositeFormat.Parse(Messages.UnexpectedDateFormat);
    private static readonly CompositeFormat UnexpectedAmountFormat = CompositeFormat.Parse(Messages.UnexpectedAmountFormat);
    private static readonly CompositeFormat UnexpectedRemainingAmountFormat = CompositeFormat.Parse(Messages.UnexpectedRemainingAmountFormat);

    /// <inheritdoc/>
    public void ParseStatement(Statement<DebitTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        DebitStatement debitStatement = statement as DebitStatement
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

        if (row.ChildElements.Count < 5)
            throw new ParserException(FormatMessage(InsufficientRowNumber, row.ChildElements.Count));

        string dateString = row.GetCell(0).GetCellValue(stringTable);

        if (string.IsNullOrWhiteSpace(dateString))
            throw new ParserException(FormatMessage(TransactionDateColumnMissing, order + 1));

        if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
            throw new ParserException(FormatMessage(UnexpectedDateFormat, dateString, order + 1));

        if (!row.GetCell(2).TryGetDecimalValue(out decimal? amountValue))
            throw new ParserException(FormatMessage(UnexpectedAmountFormat, order + 1));

        if (!row.GetCell(3).TryGetDecimalValue(out decimal? remainingBalanceValue))
            throw new ParserException(FormatMessage(UnexpectedRemainingAmountFormat, order + 1));

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

    private static string FormatMessage(CompositeFormat format, params object[] parameters)=>
        string.Format(CultureInfo.InvariantCulture, format, parameters);


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
