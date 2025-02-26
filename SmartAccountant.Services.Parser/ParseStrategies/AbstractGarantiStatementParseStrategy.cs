using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Helpers;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal abstract class AbstractGarantiStatementParseStrategy
{
    protected static readonly CompositeFormat InsufficientColumnCount = CompositeFormat.Parse(Messages.InsufficientColumnCount);
    protected static readonly CompositeFormat TransactionDateColumnMissing = CompositeFormat.Parse(Messages.TransactionDateColumnMissing);
    protected static readonly CompositeFormat UnexpectedDateFormat = CompositeFormat.Parse(Messages.UnexpectedDateFormat);
    protected static readonly CompositeFormat UnexpectedAmountFormat = CompositeFormat.Parse(Messages.UnexpectedAmountFormat);

    /// <exception cref="ParserException"/>
    protected static void VerifyColumnCount(Row row, int expectedCount)
    {
        if (row.ChildElements.Count < expectedCount)
            throw new ParserException(FormatMessage(InsufficientColumnCount, row.ChildElements.Count));
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentNullException"/>
    protected static DateTimeOffset ParseDate(Row row, int column, SharedStringTable stringTable, short order)
    {
        string dateString = row.GetCell(column).GetCellValue(stringTable);

        if (string.IsNullOrWhiteSpace(dateString))
            throw new ParserException(FormatMessage(TransactionDateColumnMissing, order + 1));

        if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
            throw new ParserException(FormatMessage(UnexpectedDateFormat, dateString, order + 1));

        return new DateTimeOffset(date, TimeSpan.Zero);
    }

    /// <exception cref="ArgumentOutOfRangeException"/>
    protected static bool ParseMoney(Row row, int column, Currency currency, [NotNullWhen(true)] out MonetaryValue? value)
    {
        if (!row.GetCell(column).TryGetDecimalValue(out decimal? amountValue))
        {
            value = null;
            return false;
        }

        value = new(amountValue.Value.Round(), currency);

        return true;
    }

    protected static string FormatMessage(CompositeFormat format, params object[] parameters) =>
        string.Format(CultureInfo.InvariantCulture, format, parameters);
}
