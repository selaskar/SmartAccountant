using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Import.Service.Tests.DebitImportServiceTests;

[TestClass]
public class Parse : Base
{
    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementFactory()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            File = null!
        };

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws<NotImplementedException>();

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(ImportErrors.CannotParseUploadedStatementFile, result.Error);
    }

    [TestMethod]
    public async Task DirectlyThrowImportExceptionInStatementFactory()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            File = null!
        };

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws(new ImportException(ImportErrors.Unspecified, "test"));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForParserExceptionInParser()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            File = null!
        };

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, null!);

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(ImportErrors.CannotParseUploadedStatementFile, result.Error);
    }

    [TestMethod]
    public async Task SucceedForZeroTransaction()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        DebitStatement statement = new();

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        // Act
        Statement result = await sut.Parse(model, account, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);
    }

    [TestMethod]
    public async Task Succeed()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        DebitStatement statement = new()
        {
            Transactions = [new DebitTransaction()
            {
                Amount= new MonetaryValue(100, Currency.USD),
                Description = "",
                RemainingBalance = new MonetaryValue(15, Currency.USD),
            }]
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        // Act
        Statement result = await sut.Parse(model, account, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);
    }
}
