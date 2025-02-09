using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;

namespace SmartAccountant.Services.Parser;

internal class ExcelSpreadsheetParserService(IStatementParseStrategyFactory factory) : ISpreadsheetParser
{
    /// <inheritdoc />
    public void ReadStatement<TTransaction>(Statement<TTransaction> statement, Stream stream)
         where TTransaction : Transaction
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

            IStatementParseStrategy<TTransaction> statementParseStrategy = factory.Create<TTransaction>(statement.Account.Bank);
            statementParseStrategy.ParseStatement(statement, worksheet, sharedStringTable);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ParserException)
        {
            throw new ParserException("An unexpected error occurred while parsing the spreadsheet.", ex);
        }
    }
}
