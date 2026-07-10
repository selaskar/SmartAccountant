using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Helpers;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal abstract class AbstractGarantiStatementParseStrategy
{
    protected static readonly CompositeFormat UnexpectedAmountFormat = CompositeFormat.Parse(Messages.UnexpectedAmountFormat);

    private static readonly CompositeFormat InsufficientColumnCount = CompositeFormat.Parse(Messages.InsufficientColumnCount);
    private static readonly CompositeFormat TransactionDateColumnMissing = CompositeFormat.Parse(Messages.TransactionDateColumnMissing);
    private static readonly CompositeFormat UnexpectedDateFormat = CompositeFormat.Parse(Messages.UnexpectedDateFormat);

    /// <exception cref="ParserException"/>
    private protected static void VerifyColumnCount(Row row, int expectedCount)
    {
        if (row.ChildElements.Count < expectedCount)
            throw new ParserException(ParserErrors.InsufficientColumnCount, InsufficientColumnCount.FormatMessage(row.ChildElements.Count));
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="OverflowException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private protected static DateTimeOffset ParseDate(Row row, int column, SharedStringTable stringTable, short order)
    {
        string dateString = row.GetCell(column).GetCellValue(stringTable);

        if (string.IsNullOrWhiteSpace(dateString))
            throw new ParserException(ParserErrors.TransactionDateColumnMissing, TransactionDateColumnMissing.FormatMessage(order + 1));

        if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
            throw new ParserException(ParserErrors.UnexpectedDateFormat, UnexpectedDateFormat.FormatMessage(dateString, order + 1));

        return new DateTimeOffset(date, TimeSpan.Zero);
    }

    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private protected static bool ParseMoney(Row row, int column, Currency currency, [NotNullWhen(true)] out MonetaryValue? value)
    {
        if (!row.GetCell(column).TryGetDecimalValue(out decimal? amountValue))
        {
            value = null;
            return false;
        }

        value = new(amountValue.Value.Round(), currency);

        return true;
    }

    /// <exception cref="ArgumentNullException"/>
    private protected static bool ParseMoney(Row row, int column, Currency currency, decimal defaultIfEmpty, SharedStringTable stringTable, IFormatProvider formatProvider, [NotNullWhen(true)] out MonetaryValue? value)
    {
        if (string.IsNullOrWhiteSpace(row.GetCell(column).InnerText))
        {
            value = new(defaultIfEmpty, currency);
            return true;
        }

        if (!row.GetCell(column).TryGetDecimalValue(stringTable, formatProvider, out decimal? amountValue))
        {
            value = null;
            return false;
        }

        value = new(amountValue.Value.Round(), currency);

        return true;
    }

    /// <exception cref="InvalidCastException"/>
    private protected static TStatement Cast<TTransaction, TStatement>(IStatement<TTransaction> statement)
        where TTransaction : Transaction
        where TStatement : class, IStatement<TTransaction>
    {
        return statement as TStatement
            //TODO: update exception message
            ?? throw new InvalidCastException($"Statement (type: {statement.GetType().Name}) was expected to be type of {typeof(TStatement).Name}.");
    }
}
