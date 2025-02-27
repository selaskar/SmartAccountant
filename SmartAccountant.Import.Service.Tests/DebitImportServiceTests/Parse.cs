﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service.Tests.DebitImportServiceTests;

[TestClass]
public class Parse
{
    private Mock<ILogger<DebitImportService>> loggerMock = null!;
    private Mock<IFileTypeValidator> fileTypeValidator = null!;
    private Mock<IAuthorizationService> authorizationServiceMock = null!;
    private Mock<IAccountRepository> accountRepositoryMock = null!;
    private Mock<IStorageService> storageServiceMock = null!;
    private Mock<IStatementRepository> statementRepositoryMock = null!;
    private Mock<IValidator<DebitStatementImportModel>> validatorMock = null!;
    private Mock<IStatementFactory> statementFactoryMock = null!;
    private Mock<ISpreadsheetParser> parserMock = null!;

    private DebitImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new Mock<ILogger<DebitImportService>>();
        fileTypeValidator = new Mock<IFileTypeValidator>();
        authorizationServiceMock = new Mock<IAuthorizationService>();
        accountRepositoryMock = new Mock<IAccountRepository>();
        storageServiceMock = new Mock<IStorageService>();
        statementRepositoryMock = new Mock<IStatementRepository>();
        validatorMock = new Mock<IValidator<DebitStatementImportModel>>();
        statementFactoryMock = new Mock<IStatementFactory>();
        parserMock = new Mock<ISpreadsheetParser>();

        sut = new(
            loggerMock.Object,
            fileTypeValidator.Object,
            authorizationServiceMock.Object,
            accountRepositoryMock.Object,
            storageServiceMock.Object,
            statementRepositoryMock.Object,
            validatorMock.Object,
            statementFactoryMock.Object,
            parserMock.Object);
    }

    [TestMethod]
    public void ThrowImportExceptionForWrongModelType()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
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
        var result = Assert.ThrowsExactly<ImportException>(() => sut.Parse(model, account));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public void DirectlyThrowImportExceptionInStatementFactory()
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
        var result = Assert.ThrowsExactly<ImportException>(() => sut.Parse(model, account));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }


    [TestMethod]
    public void Succeed()
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
        Statement result = sut.Parse(model, account);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);
    }


    private void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private void SetupStatementFactory(DebitStatementImportModel request, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(request, account)).Returns(statement);
}
