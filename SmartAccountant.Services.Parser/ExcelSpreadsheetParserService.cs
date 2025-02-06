using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;

namespace SmartAccountant.Services.Parser;

public class ExcelSpreadsheetParserService(IStatementParseStrategyFactory factory) : ISpreadsheetParser
{
    /// <inheritdoc />
    public void ReadStatement(Statement statement, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(statement);
        ArgumentNullException.ThrowIfNull(statement.Account);

        try
        {
            using var document = SpreadsheetDocument.Open(stream, false);

            string sheetId = document.WorkbookPart?.Workbook.Descendants<Sheet>().FirstOrDefault()?.Id?.Value
                ?? throw new ParserException("Document does not contain a sheet.");

            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart!.GetPartById(sheetId);
            Worksheet worksheet = worksheetPart.Worksheet;

            SharedStringTable sharedStringTable = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable;

            IStatementParseStrategy statementParseStrategy = factory.Create(statement.Account.Bank);

            switch (statement.Account.NormalBalance)
            {
                case BalanceType.Debit:
                    statementParseStrategy.ParseDebitStatement(statement, worksheet, sharedStringTable);
                    break;
                case BalanceType.Credit:
                    statementParseStrategy.ParseCreditStatement(statement, worksheet, sharedStringTable);
                    break;
                default:
                    throw new NotImplementedException($"Balance type ({statement.Account.NormalBalance}) is not implemented yet.");
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ParserException)
        {
            throw new ParserException("An unexpected error occurred while parsing the spreadsheet.", ex);
        }
    }
}
