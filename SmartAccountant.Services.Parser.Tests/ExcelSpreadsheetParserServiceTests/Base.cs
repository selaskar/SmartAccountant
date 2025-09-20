using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SmartAccountant.Services.Parser.Tests.ExcelSpreadsheetParserServiceTests;

public abstract class Base
{
    private protected static MemoryStream GetValidSpreadsheet()
    {
        var stream = new MemoryStream();

        using var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        //work book
        WorkbookPart workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        //work sheet
        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());
        workbookPart.Workbook.AppendChild(new Sheets(
            new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            }));

        //string table
        var sharedStringTablePart = workbookPart.AddNewPart<SharedStringTablePart>();
        sharedStringTablePart.SharedStringTable = new SharedStringTable();

        document.Save();

        stream.Position = 0;

        return stream;
    }
}
