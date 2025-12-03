using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed partial class GarantiMultipartStatementParseStrategy : AbstractGarantiCreditCardStatementParseStrategy,
    IMultipartStatementParseStrategy
{
    /// <summary>
    /// This is the gap from card number row to first transaction in that section.
    /// </summary>
    internal const int HeaderRowCount = 2;
    internal const string FeeKeyword = "ücret";
    internal const decimal DeflectionIgnoreThreshold = 40m;

    private static readonly CompositeFormat UnexpectedPartCount = CompositeFormat.Parse(Messages.UnexpectedPartCount);
    private static readonly CompositeFormat DeflectionTooLarge = CompositeFormat.Parse(Messages.DeflectionTooLarge);

    /// <inheritdoc />
    public void ParseMultipartStatement(SharedStatement statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        Row[] rows = worksheet.Descendants<Row>().ToArray();

        Regex regex = CardNumberPattern();

        ImmutableArray<Row> sectionHeaders = rows.Where(row => row.ChildElements.Count == 1 && regex.IsMatch(row.GetCell(0).GetCellValue(stringTable)))
            .ToImmutableArray();

        if (sectionHeaders.Length != 2)
            throw new ParserException(UnexpectedPartCount.FormatMessage(sectionHeaders.Length));

        statement.CardNumber1 = regex.Match(sectionHeaders[0].GetCell(0).GetCellValue(stringTable)).Value;
        statement.CardNumber2 = regex.Match(sectionHeaders[1].GetCell(0).GetCellValue(stringTable)).Value;

        int section1StartIndex = Array.IndexOf(rows, sectionHeaders[0]);
        int section2StartIndex = Array.IndexOf(rows, sectionHeaders[1]);

        Span<Row> section1Rows = rows.AsSpan()[(section1StartIndex + HeaderRowCount)..section2StartIndex];
        Span<Row> section2Rows = rows.AsSpan()[(section2StartIndex + HeaderRowCount)..];

        // We don't pass the known account number (comes from request) here,
        // because sections are sometimes in reverse order in statement.
        ParseSpan(section1Rows, null, statement.Transactions, ProvisionState.Finalized, stringTable);
        ParseSpan(section2Rows, null, statement.SecondaryTransactions, ProvisionState.Finalized, stringTable);
    }

    /// <exception cref="ParserException"/>
    public void CrossCheck(SharedStatement statement)
    {
        //TODO: will give wrong results when there are cancelled transactions.
        decimal totalExpenses = statement.Transactions.Union(statement.SecondaryTransactions)
            .Select(t => t.Amount.Amount)
            .Where(d => d > 0) //debt payments doesn't count toward total transactions.
            .DefaultIfEmpty().Sum();

        // Since we don't take cancellations into account, our sum can only be be equal to or larger than the given one in statement.
        if (statement.TotalExpenses > totalExpenses)
            throw new ParserException(Messages.TransactionAmountAndTotalExpensesDontMatch);

        CompareInfo compareInfo = new CultureInfo("tr-TR").CompareInfo;
        decimal totalFees = statement.Transactions.Union(statement.SecondaryTransactions)
            .Where(t => compareInfo.IndexOf(t.Description, FeeKeyword, CompareOptions.IgnoreCase) >= 0)
            .Select(t => t.Amount.Amount)
            .DefaultIfEmpty().Sum();

        decimal totalPayments = statement.Transactions.Union(statement.SecondaryTransactions).Select(t => t.Amount.Amount)
            .Where(d => d < 0)
            .DefaultIfEmpty().Sum();

        decimal deflection = totalPayments + totalExpenses - statement.TotalExpenses + statement.TotalPayments - totalFees;
        if (Math.Abs(deflection) > DeflectionIgnoreThreshold)
            throw new ParserException(DeflectionTooLarge.FormatMessage(deflection, DeflectionIgnoreThreshold));
    }

    // Example: 1234 **** **** 5678
    [GeneratedRegex("^\\d{4}[ *]{11}\\d{4}")]
    private static partial Regex CardNumberPattern();
}
