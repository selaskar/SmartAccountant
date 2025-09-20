using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser;

internal class ExcelSpreadsheetParserService(
    IStatementParseStrategyFactory factory, 
    IMultipartStatementParseStrategy multipartStatementParseStrategy) : 
    ISpreadsheetParser, IMultipartStatementParser
{
    /// <inheritdoc />
    public void ReadStatement<TTransaction>(Statement<TTransaction> statement, Stream stream, Bank bank)
         where TTransaction : Transaction
    {
        try
        {
            (Worksheet worksheet, SharedStringTable sharedStringTable) = ParseCommon(stream);

            IStatementParseStrategy<TTransaction> statementParseStrategy = factory.Create<TTransaction>(bank);
            statementParseStrategy.ParseStatement(statement, worksheet, sharedStringTable);
            statementParseStrategy.CrossCheck(statement);
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedErrorParsingStatement, ex);
        }
    }

    /// <inheritdoc />
    public void ReadMultipartStatement(SharedStatement statement, Stream stream, Bank bank)
    {
        try
        {
            (Worksheet worksheet, SharedStringTable sharedStringTable) = ParseCommon(stream);

            multipartStatementParseStrategy.ParseMultipartStatement(statement, worksheet, sharedStringTable);
            multipartStatementParseStrategy.CrossCheck(statement);
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedErrorParsingStatement, ex);
        }
    }

    /// <exception cref="ParserException"/>
    private static (Worksheet, SharedStringTable) ParseCommon(Stream stream)
    {
        try
        {
            using var document = SpreadsheetDocument.Open(stream, false);

            string sheetPartId = document.WorkbookPart?.Workbook?.Descendants<Sheet>().FirstOrDefault()?.Id?.Value
                ?? throw new ParserException(Messages.UploadedDocumentMissingSheet);

            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart!.GetPartById(sheetPartId);
            Worksheet worksheet = worksheetPart.Worksheet;

            SharedStringTable sharedStringTable = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable;

            return (worksheet, sharedStringTable);
        }
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedErrorParsingSpreadsheet, ex);
        }
    }
}
