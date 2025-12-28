using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Services.Parser.Tests.ExcelSpreadsheetParserServiceTests;

[TestClass]
public class ReadMultipartStatement : Base
{
    private Mock<IMultipartStatementParseStrategy> mockParseStrategy = null!;

    private ExcelSpreadsheetParserService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        mockParseStrategy = new();

        sut = new(null!, mockParseStrategy.Object);
    }

    [TestMethod]
    public void ThrowParserExceptionForInvalidStream()
    {
        // Arrange
        SharedStatement statement = new();

        MemoryStream stream = null!; // will throw the error

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.ReadMultipartStatement(null!, stream, Bank.GarantiBBVA));

        Assert.AreEqual(Messages.UnexpectedErrorParsingSpreadsheet, result.Message);
    }

    [TestMethod]
    public void ReThrowParserExceptionForFailingCrossCheck()
    {
        // Arrange
        SharedStatement statement = new();

        using MemoryStream stream = GetValidSpreadsheet();

        mockParseStrategy.Setup(x => x.CrossCheck(statement)).Throws(() => new ParserException(ParserErrors.Unspecified));

        // Act, Assert
        var result = Assert.ThrowsExactly<ParserException>(() => sut.ReadMultipartStatement(statement, stream, Bank.GarantiBBVA));

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public void ThrowParserExceptionForUnexpectedError()
    {
        // Arrange
        SharedStatement statement = new();

        using MemoryStream stream = GetValidSpreadsheet();

        mockParseStrategy.Setup(x => x.CrossCheck(statement)).Throws<InvalidOperationException>();

        // Act, Assert
        Assert.ThrowsExactly<ParserException>(() => sut.ReadMultipartStatement(statement, stream, Bank.GarantiBBVA));
    }

    [TestMethod]
    public void CallParseStatementForValidInput()
    {
        // Arrange
        SharedStatement statement = new();

        using MemoryStream stream = GetValidSpreadsheet();

        // Act
        sut.ReadMultipartStatement(statement, stream, Bank.GarantiBBVA);

        // Assert
        mockParseStrategy.Verify(s => s.ParseMultipartStatement(statement, It.IsAny<Worksheet>(), It.IsAny<SharedStringTable>()), Times.Once);
        mockParseStrategy.Verify(s => s.CrossCheck(statement), Times.Once);
    }
}
