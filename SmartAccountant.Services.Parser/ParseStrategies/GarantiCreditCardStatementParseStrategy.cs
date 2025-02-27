using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed class GarantiCreditCardStatementParseStrategy : AbstractGarantiStatementParseStrategy, IStatementParseStrategy<CreditCardTransaction>
{
    /// Non-empty rows
    internal const int HeaderRowCount = 3;

    public void ParseStatement(Statement<CreditCardTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        var creditCardStatement = statement as CreditCardStatement
            ?? throw new ArgumentException($"Statement expected to be type of {typeof(CreditCardStatement).Name}.");

        try
        {
            Row[] rows = [.. worksheet.Descendants<Row>()];
            int transactionCount = rows.Length - HeaderRowCount - 1; //Last row is sum.
            short rowCount = 0;
            foreach (Row row in rows.Skip(HeaderRowCount).Take(transactionCount))
            {
                CreditCardTransaction transaction = ParseCreditCardTransaction(creditCardStatement, rowCount++, row, stringTable);
                statement.Transactions.Add(transaction);
            }
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedError, ex);
        }

        CrossCheck(creditCardStatement);
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private static CreditCardTransaction ParseCreditCardTransaction(CreditCardStatement statement, short order, Row row, SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Label (2), Bonus (3), Amount (4)

        VerifyColumnCount(row, 5);

        DateTimeOffset date = ParseDate(row, column: 0, stringTable, order);

        if (!ParseMoney(row, column: 4, Currency.TRY, out MonetaryValue? amount))
            throw new ParserException(FormatMessage(UnexpectedAmountFormat, order + 1));

        return new CreditCardTransaction()
        {
            Id = Guid.NewGuid(),
            StatementId = statement.Id,
            Timestamp = date,
            Amount = amount.Value * -1, // Normal balance is credit
            ReferenceNumber = null,
            Description = row.GetCell(1).GetCellValue(stringTable),
        };
    }

    /// <exception cref="ParserException"/>
    private static void CrossCheck(CreditCardStatement statement)
    {
        decimal expectedSum = statement.TotalDueAmount - statement.RolloverAmount ?? 0;

        decimal totalTransactions = !statement.Transactions.Any() ? 0
            : statement.Transactions.Sum(t => t.Amount.Amount);

        if (expectedSum != totalTransactions)
            throw new ParserException(Messages.TransactionAmountAndDueAmountNotMatch);
    }
}
