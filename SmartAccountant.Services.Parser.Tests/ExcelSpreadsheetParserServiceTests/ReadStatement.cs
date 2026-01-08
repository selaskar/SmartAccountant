using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Services.Parser.Tests.ExcelSpreadsheetParserServiceTests;

[TestClass]
public class ReadStatement : Base
{
    private Mock<IStatementParseStrategyFactory> mockFactory = null!;

    private ExcelSpreadsheetParserService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        mockFactory = new();

        sut = new(mockFactory.Object, null!);
    }

    [TestMethod]
    public void ThrowParserExceptionForMissingSpreadsheetParts()
    {
        // Arrange
        DebitStatement statement = new();

        using MemoryStream stream = new();

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
            Assert.AreEqual(ParserErrors.UploadedDocumentMissingSheet, exception.Error);
        }
    }

    [TestMethod]
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        DebitStatement statement = new();

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
        DebitStatement statement = new();

        using MemoryStream stream = GetValidSpreadsheet();

        Mock<IStatementParseStrategy<DebitTransaction>> mockStrategy = new();
        mockFactory.Setup(f => f.Create<DebitTransaction>(Bank.GarantiBBVA)).Returns(mockStrategy.Object);

        // Act
        sut.ReadStatement(statement, stream, Bank.GarantiBBVA);

        // Assert
        mockStrategy.Verify(s => s.ParseStatement(statement, It.IsAny<Worksheet>(), It.IsAny<SharedStringTable>()), Times.Once);
    }

    [TestMethod]
    public void ReThrowParserExceptionForFailingCrossCheck()
    {
        // Arrange
        DebitStatement statement = new();

        using MemoryStream stream = GetValidSpreadsheet();

        Mock<IStatementParseStrategy<DebitTransaction>> mockStrategy = MockStatementFactory();

        mockStrategy.Setup(x => x.CrossCheck(statement)).Throws(() => new ParserException(ParserErrors.DeflectionTooLarge));

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.ReadStatement(statement, stream, Bank.GarantiBBVA));

        Assert.AreEqual(ParserErrors.DeflectionTooLarge, result.Error);
    }


    private Mock<IStatementParseStrategy<DebitTransaction>> MockStatementFactory()
    {
        Mock<IStatementParseStrategy<DebitTransaction>> mockStrategy = new();
        mockFactory.Setup(f => f.Create<DebitTransaction>(Bank.GarantiBBVA)).Returns(mockStrategy.Object);
        return mockStrategy;
    }
}
