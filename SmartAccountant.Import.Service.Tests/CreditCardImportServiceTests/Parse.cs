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
    public void ThrowImportExceptionForWrongModelType()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            File = null!
        };

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => sut.Parse(model, null!));
    }

    [TestMethod]
    public void ThrowImportExceptionForUnexpectedErrorInStatementFactory()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
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
        var result = Assert.ThrowsExactly<ImportException>(() => sut.Parse(model, account));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public void DirectlyThrowImportExceptionInStatementFactory()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = null!
        };

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws(new ImportException("test"));

        // Act, Assert
        var result = Assert.ThrowsExactly<ImportException>(() => sut.Parse(model, account));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public void ThrowImportExceptionForParserExceptionInParser()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
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
        var result = Assert.ThrowsExactly<ImportException>(() => sut.Parse(model, account));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public void Succeed()
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

        SavingAccount account = new()
        {
            AccountNumber = "0"
        };

        CreditCardStatement statement = new();

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        // Act
        Statement result = sut.Parse(model, account);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);
    }
}
