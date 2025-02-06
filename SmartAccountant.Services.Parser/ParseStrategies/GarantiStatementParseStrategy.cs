using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;

namespace SmartAccountant.Services.Parser.ParseStrategies;

//TODO: parsing culture and date style
internal sealed class GarantiStatementParseStrategy : IStatementParseStrategy
{
    /// <inheritdoc/>
    public void ParseDebitStatement(Statement statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        foreach (Row row in worksheet.Descendants<Row>().Skip(11))
            statement.Transactions.Add(ParseDebitTransaction(statement, row, stringTable));
    }

    /// <inheritdoc/>
    public void ParseCreditStatement(Statement statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        throw new NotImplementedException();
    }


    /// <exception cref="ParserException"/>
    private static DebitTransaction ParseDebitTransaction(Statement statement, Row row, SharedStringTable stringTable)
    {
        // Expected row format: Date (0), Description (1), Amount (2), Remaining Balance (3), Reference Number (4)

        if (row.ChildElements.Count < 5)
            throw new ParserException($"Unrecognized file format. Column count ({row.ChildElements.Count}) was expected to be at least 5.");

        string dateString = row.GetCellValue(0, stringTable);

        if (string.IsNullOrWhiteSpace(dateString))
            throw new ParserException("Unrecognized file format. Expected to have transaction date at column A.");

        if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
            throw new ParserException($"Unrecognized file format. Could not parse transaction date '{dateString}'.");


        MonetaryValue amount = new(ParseDecimal(row.GetCellValue(2, stringTable)), Currency.TRY);
        MonetaryValue remainingBalance = new(ParseDecimal(row.GetCellValue(3, stringTable)), Currency.TRY);

        return new DebitTransaction
        {
            Id = Guid.NewGuid(),
            StatementId = statement.Id,
            Timestamp = date,
            Amount = amount,
            RemainingBalance = remainingBalance,
            ReferenceNumber = row.GetCellValue(4, stringTable),
            Note = row.GetCellValue(1, stringTable)
        };
    }

    private static decimal ParseDecimal(string value) => Math.Round(decimal.Parse(value, CultureInfo.InvariantCulture), 2, MidpointRounding.AwayFromZero);
}
