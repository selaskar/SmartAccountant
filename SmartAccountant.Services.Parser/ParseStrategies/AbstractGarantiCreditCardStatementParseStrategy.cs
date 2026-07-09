using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal abstract class AbstractGarantiCreditCardStatementParseStrategy<TTransaction> : AbstractGarantiStatementParseStrategy,
    ISpreadsheetParseStrategy<TTransaction>
    where TTransaction : CreditCardTransaction
{
    protected static readonly CultureInfo ParseCulture = CultureInfo.GetCultureInfo("tr-TR");

    public abstract void CrossCheck(IStatement<TTransaction> statement);
    public abstract void ParseStatement(IStatement<TTransaction> statement, Worksheet worksheet, SharedStringTable stringTable);

    /// <exception cref="ParserException"/>
    /// <exception cref="ServerException"/>
    private protected void ParseSpan(ReadOnlySpan<Row> rowsSpan, Guid? accountId, IList<TTransaction> transactions, ProvisionState provisionState, SharedStringTable stringTable)
    {
        try
        {
            short rowNumber = 0;
            foreach (Row row in rowsSpan)
            {
                TTransaction transaction = ParseTransaction(accountId, rowNumber++, provisionState, row, stringTable);
                transactions.Add(transaction);
            }
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentOutOfRangeException or FormatException or OverflowException)
        {
            throw new ParserException(ParserErrors.UnexpectedCreditCardStatementFormat, ex);
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ServerException(Messages.UnexpectedErrorParsingStatement, ex);
        }
    }


    /// <exception cref="ParserException"/>
    /// <exception cref="OverflowException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private TTransaction ParseTransaction(
        Guid? accountId,
        short rowNumber,
        ProvisionState provisionState,
        Row row,
        SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Label (2), Bonus (3), Amount (4)

        VerifyColumnCount(row, 5);

        DateTimeOffset date = ParseDate(row, column: 0, stringTable, rowNumber);

        if (!ParseMoney(row, column: 4, Currency.TRY, defaultIfEmpty: 0, stringTable, ParseCulture, out MonetaryValue? amount))
            throw new ParserException(ParserErrors.UnexpectedAmountFormat, UnexpectedAmountFormat.FormatMessage(rowNumber + 1));

        return CreateTransaction(accountId, date, amount.Value, row, stringTable, provisionState);
    }


    /// <exception cref="OverflowException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private protected abstract TTransaction CreateTransaction(
        Guid? accountId,
        DateTimeOffset date,
        MonetaryValue amount,
        Row row,
        SharedStringTable stringTable,
        ProvisionState provisionState);
}
