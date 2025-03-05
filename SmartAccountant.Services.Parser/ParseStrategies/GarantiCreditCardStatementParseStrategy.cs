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
    internal const int OpenProvisionHeaderRowCount = 3;
    internal const int FooterRowCount = 1;

    internal const string OpenProvisionLabel = "Açık Provizyon - TL";
    internal const string OpenProvisionTotalAmountLabel = "Toplam Açık Provizyon:";
    internal const string RegularTransactionsLabel = "Dönemiçi İşlemler - TL";

    /// <inheritdoc/>
    public void ParseStatement(Statement<CreditCardTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        var creditCardStatement = statement as CreditCardStatement
            ?? throw new ArgumentException($"Statement expected to be type of {typeof(CreditCardStatement).Name}.");

        Row[] rows = [.. worksheet.Descendants<Row>()];

        Parse(creditCardStatement, rows, stringTable);

        CrossCheck(creditCardStatement);
    }

    /// <exception cref="ParserException"/>
    private static void Parse(CreditCardStatement statement, Row[] rows, SharedStringTable stringTable)
    {
        try
        {
            if (stringTable.InnerText.Contains(OpenProvisionLabel, StringComparison.InvariantCultureIgnoreCase)
                && stringTable.InnerText.Contains(RegularTransactionsLabel, StringComparison.InvariantCultureIgnoreCase))
            {
                int openProvisionRowCount = rows.Skip(HeaderRowCount)
                    .TakeWhile(r => !string.Equals(OpenProvisionTotalAmountLabel, r.GetCell(0).GetCellValue(stringTable), StringComparison.OrdinalIgnoreCase))
                    .Count();

                // Reading open provision transactions
                ParseSpan(rows.AsSpan().Slice(OpenProvisionHeaderRowCount, openProvisionRowCount),
                    statement,
                    ProvisionState.Open,
                    stringTable);

                // Reading regular transactions
                ParseSpan(rows.AsSpan()[(OpenProvisionHeaderRowCount + openProvisionRowCount + HeaderRowCount)..^FooterRowCount],
                    statement,
                    ProvisionState.Finalized,
                    stringTable);
            }
            else
            {
                if (rows.Length <= HeaderRowCount + FooterRowCount)
                    return;

                ParseSpan(rows.AsSpan()[HeaderRowCount..^FooterRowCount], statement, ProvisionState.Finalized, stringTable);
            }
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedError, ex);
        }
    }

    /// <exception cref="ParserException"/>
    private static void ParseSpan(
        ReadOnlySpan<Row> rowsSpan,
        CreditCardStatement statement,
        ProvisionState provisionState,
        SharedStringTable stringTable)
    {
        try
        {
            short rowNumber = 0;
            foreach (Row row in rowsSpan)
            {
                CreditCardTransaction transaction = ParseCreditCardTransaction(statement, rowNumber++, provisionState, row, stringTable);
                statement.Transactions.Add(transaction);
            }
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedError, ex);
        }
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private static CreditCardTransaction ParseCreditCardTransaction(
        CreditCardStatement statement,
        short rowNumber,
        ProvisionState provisionState,
        Row row,
        SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Label (2), Bonus (3), Amount (4)

        VerifyColumnCount(row, 5);

        DateTimeOffset date = ParseDate(row, column: 0, stringTable, rowNumber);

        if (!ParseMoney(row, column: 4, Currency.TRY, defaultIfEmpty: 0, out MonetaryValue? amount))
            throw new ParserException(FormatMessage(UnexpectedAmountFormat, rowNumber + 1));

        return new CreditCardTransaction()
        {
            Id = Guid.NewGuid(),
            AccountId = statement.AccountId,
            Timestamp = date,
            Amount = amount.Value * -1, //Since the normal balance is credit
            ReferenceNumber = null,
            Description = row.GetCell(1).GetCellValue(stringTable),
            ProvisionState = provisionState
        };
    }

    /// <exception cref="ParserException"/>
    private static void CrossCheck(CreditCardStatement statement)
    {
        decimal expectedSum = statement.TotalDueAmount - (statement.RolloverAmount ?? 0);

        decimal totalTransactions = !statement.Transactions.Any() ? 0
            : statement.Transactions.Sum(t => t.Amount.Amount);

        if (expectedSum != totalTransactions)
            throw new ParserException(Messages.TransactionAmountAndDueAmountNotMatch);
    }
}
