using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Services.Parser;

internal class ExcelSpreadsheetParserService(ISpreadsheetParseStrategyFactory factory) :
    ISpreadsheetParser
{
    /// <inheritdoc />
    public void ReadStatement<TTransaction>(IStatement<TTransaction> statement, Stream stream, Bank bank)
         where TTransaction : Transaction
    {
        try
        {
            (Worksheet worksheet, SharedStringTable sharedStringTable) = ParseCommon(stream);

            ISpreadsheetParseStrategy<TTransaction> spreadsheetParseStrategy = factory.Create<TTransaction>(bank);
            spreadsheetParseStrategy.ParseStatement(statement, worksheet, sharedStringTable);
            spreadsheetParseStrategy.CrossCheck(statement);
        }
        catch (ArgumentNullException ex)
        {
            throw new ParserException(ParserErrors.CouldNotReadStatement, ex);
        }
        catch (Exception ex) when (ex is not ServerException and not ParserException)
        {
            throw new ServerException(Messages.UnexpectedErrorReadingStatement, ex);
        }
    }

    /// <exception cref="ParserException"/>
    /// <exception cref="ServerException"/>
    private static (Worksheet, SharedStringTable) ParseCommon(Stream stream)
    {
        try
        {
            using var document = SpreadsheetDocument.Open(stream, false);

            string sheetPartId = document.WorkbookPart?.Workbook?.Descendants<Sheet>().FirstOrDefault()?.Id?.Value
                ?? throw new ParserException(ParserErrors.UploadedDocumentMissingSheet);

            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart!.GetPartById(sheetPartId);
            Worksheet worksheet = worksheetPart.Worksheet;

            SharedStringTable sharedStringTable = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable;

            return (worksheet, sharedStringTable);
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentOutOfRangeException)
        {
            throw new ParserException(ParserErrors.CouldNotReadStatement, ex);
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ServerException(Messages.UnexpectedErrorParsingSpreadsheet, ex);
        }
    }
}
