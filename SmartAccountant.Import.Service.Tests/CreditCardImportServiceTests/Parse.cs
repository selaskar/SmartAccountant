using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service.Tests.CreditCardImportServiceTests;

[TestClass]
public class Parse
{
    private Mock<ILogger<CreditCardImportService>> loggerMock = null!;
    private Mock<IFileTypeValidator> fileTypeValidator = null!;
    private Mock<IAuthorizationService> authorizationServiceMock = null!;
    private Mock<IAccountRepository> accountRepositoryMock = null!;
    private Mock<IStorageService> storageServiceMock = null!;
    private Mock<ITransactionRepository> transactionRepositoryMock = null!;
    private Mock<IStatementRepository> statementRepositoryMock = null!;
    private Mock<IValidator<CreditCardStatementImportModel>> validatorMock = null!;
    private Mock<IStatementFactory> statementFactoryMock = null!;
    private Mock<ISpreadsheetParser> parserMock = null!;

    private CreditCardImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new Mock<ILogger<CreditCardImportService>>();
        fileTypeValidator = new Mock<IFileTypeValidator>();
        authorizationServiceMock = new Mock<IAuthorizationService>();
        accountRepositoryMock = new Mock<IAccountRepository>();
        storageServiceMock = new Mock<IStorageService>();
        transactionRepositoryMock = new Mock<ITransactionRepository>();
        statementRepositoryMock = new Mock<IStatementRepository>();
        validatorMock = new Mock<IValidator<CreditCardStatementImportModel>>();
        statementFactoryMock = new Mock<IStatementFactory>();
        parserMock = new Mock<ISpreadsheetParser>();

        sut = new(
            loggerMock.Object,
            fileTypeValidator.Object,
            authorizationServiceMock.Object,
            accountRepositoryMock.Object,
            storageServiceMock.Object,
            transactionRepositoryMock.Object,
            statementRepositoryMock.Object,
            validatorMock.Object,
            statementFactoryMock.Object,
            parserMock.Object);
    }

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


    private void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private void SetupStatementFactory(CreditCardStatementImportModel request, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(request, account)).Returns(statement);
}
