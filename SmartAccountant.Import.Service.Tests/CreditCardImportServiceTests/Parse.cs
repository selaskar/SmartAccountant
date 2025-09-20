using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.CreditCardImportServiceTests;

[TestClass]
public class Parse : Base
{
    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementFactory()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = null!
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws<NotImplementedException>();

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task DirectlyThrowImportExceptionInStatementFactory()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = null!
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws(new ImportException("test"));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForParserExceptionInParser()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = null! //throws the exception
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, null!);

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task Succeed()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        CreditCardStatement statement = new();

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        // Act
        Statement result = await sut.Parse(model, account, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);
    }
}
