using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.Tests.ExcelSpreadsheetParserServiceTests;

[TestClass]
public class ReadStatement
{
    private Mock<IStatementParseStrategyFactory> mockFactory = null!;

    private ExcelSpreadsheetParserService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        mockFactory = new Mock<IStatementParseStrategyFactory>();
        sut = new ExcelSpreadsheetParserService(mockFactory.Object);
    }

    [TestMethod]
    public void ThrowParserExceptionForMissingSpreadsheetParts()
    {
        // Arrange
        var statement = new DebitStatement();

        using var stream = new MemoryStream();

        //Checking for missing workbook part
        using var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        document.Save();
        stream.Position = 0;
        AssertParserException();

        //Checking for missing workbook
        WorkbookPart workbookPart = document.AddWorkbookPart();
        document.Save();
        stream.Position = 0;
        AssertParserException();

        //Checking for missing sheet
        workbookPart.Workbook = new Workbook();
        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());
        document.Save();
        stream.Position = 0;
        AssertParserException();

        //Checking for missing sheet id
        workbookPart.Workbook.AppendChild(new Sheets(
            new Sheet
            {
                Id = null,
            }));
        document.Save();
        stream.Position = 0;
        AssertParserException();

        void AssertParserException()
        {
            var exception = Assert.Throws<ParserException>(() => sut.ReadStatement(statement, stream, Bank.Unknown));
            Assert.AreEqual(Messages.UploadedDocumentMissingSheet, exception.Message);
        }
    }

    [TestMethod]
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        var statement = new DebitStatement();

        using MemoryStream stream = GetValidSpreadsheet();

        mockFactory.Setup(x => x.Create<Transaction>(It.IsAny<Bank>())).Throws(() => new InvalidOperationException("test"));

        // Act, Assert
        var exception = Assert.ThrowsExactly<ParserException>(() => sut.ReadStatement(statement, stream, Bank.Unknown));

        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
        Assert.AreEqual("test", exception.InnerException.Message);
    }

    [TestMethod]
    public void CallParseStatementForValidInput()
    {
        // Arrange
        var statement = new DebitStatement();

        using MemoryStream stream = GetValidSpreadsheet();

        var mockStrategy = new Mock<IStatementParseStrategy<DebitTransaction>>();
        mockFactory.Setup(f => f.Create<DebitTransaction>(Bank.GarantiBBVA)).Returns(mockStrategy.Object);

        // Act
        sut.ReadStatement(statement, stream, Bank.GarantiBBVA);

        // Assert
        mockStrategy.Verify(s => s.ParseStatement(statement, It.IsAny<Worksheet>(), It.IsAny<SharedStringTable>()), Times.Once);
    }

    private static MemoryStream GetValidSpreadsheet()
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
