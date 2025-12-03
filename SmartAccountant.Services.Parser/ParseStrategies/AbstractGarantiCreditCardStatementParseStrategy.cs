using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal class AbstractGarantiCreditCardStatementParseStrategy : AbstractGarantiStatementParseStrategy
{
    private static readonly CultureInfo parseCulture = CultureInfo.GetCultureInfo("tr-TR");

    /// <exception cref="ParserException"/>
    protected internal static void ParseSpan(ReadOnlySpan<Row> rowsSpan, Guid? accountId, IList<CreditCardTransaction> transactions, ProvisionState provisionState, SharedStringTable stringTable)
    {
        try
        {
            short rowNumber = 0;
            foreach (Row row in rowsSpan)
            {
                CreditCardTransaction transaction = ParseCreditCardTransaction(accountId, rowNumber++, provisionState, row, stringTable);
                transactions.Add(transaction);
            }
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedErrorParsingStatement, ex);
        }
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    private static CreditCardTransaction ParseCreditCardTransaction(
        Guid? accountId,
        short rowNumber,
        ProvisionState provisionState,
        Row row,
        SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Label (2), Bonus (3), Amount (4)

        VerifyColumnCount(row, 5);

        DateTimeOffset date = ParseDate(row, column: 0, stringTable, rowNumber);

        if (!ParseMoney(row, column: 4, Currency.TRY, defaultIfEmpty: 0, stringTable, parseCulture, out MonetaryValue? amount))
            throw new ParserException(UnexpectedAmountFormat.FormatMessage(rowNumber + 1));

        return new CreditCardTransaction()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Timestamp = date,
            Amount = amount.Value * -1, //Since the normal balance is credit
            ReferenceNumber = null,
            Description = row.GetCell(1).GetCellValue(stringTable),
            ProvisionState = provisionState
        };
    }
}
