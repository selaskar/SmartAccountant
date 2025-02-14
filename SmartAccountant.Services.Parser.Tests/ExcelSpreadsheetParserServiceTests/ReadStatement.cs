using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;

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
    public void ThrowArgumentNullExceptionForMissingStatementOrAccount()
    {
        // Arrange
        var statement = new DebitStatement()
        {
            Account = null
        };

        var stream = new MemoryStream();

        // Act, Assert
        Assert.ThrowsException<ArgumentNullException>(() => sut.ReadStatement<Transaction>(null!, stream));

        Assert.ThrowsException<ArgumentNullException>(() => sut.ReadStatement(statement, stream));
    }

    [TestMethod]
    public void ThrowParserExceptionForMissingSheet()
    {
        // Arrange
        var statement = new DebitStatement
        {
            Account = new SavingAccount()
            {
                Bank = Bank.Unknown,
                AccountNumber = "0"
            }
        };

        using var stream = new MemoryStream();

        using var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        document.AddWorkbookPart();
        document.Save();

        stream.Position = 0;

        // Act, Assert
        Assert.ThrowsException<ParserException>(() => sut.ReadStatement(statement, stream));
    }

    [TestMethod]
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        var statement = new DebitStatement
        {
            Account = new SavingAccount()
            {
                Bank = Bank.Unknown,
                AccountNumber = "0"
            }
        };

        using MemoryStream stream = GetValidSpreadsheet();

        mockFactory.Setup(x => x.Create<Transaction>(It.IsAny<Bank>())).Throws<Exception>();

        // Act, Assert
        Assert.ThrowsException<ParserException>(() => sut.ReadStatement(statement, stream));
    }

    [TestMethod]
    public void CallParseStatementForValidInput()
    {
        // Arrange
        var statement = new DebitStatement
        {
            Account = new SavingAccount()
            {
                Bank = Bank.GarantiBBVA,
                AccountNumber = "0"
            }
        };

        using MemoryStream stream = GetValidSpreadsheet();

        var mockStrategy = new Mock<IStatementParseStrategy<DebitTransaction>>();
        mockFactory.Setup(f => f.Create<DebitTransaction>(Bank.GarantiBBVA)).Returns(mockStrategy.Object);

        // Act
        sut.ReadStatement(statement, stream);

        // Assert
        mockStrategy.Verify(s => s.ParseStatement(statement, It.IsAny<Worksheet>(), It.IsAny<SharedStringTable>()), Times.Once);
    }

    private static MemoryStream GetValidSpreadsheet()
    {
        var stream = new MemoryStream();

        using var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        //work book
        var workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        //work sheet
        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());
        workbookPart.Workbook.AppendChild(new Sheets(new Sheet
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
