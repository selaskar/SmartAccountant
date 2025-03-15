﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service.Tests.DebitImportServiceTests;

public abstract class Base
{
    private protected Mock<ILogger<DebitImportService>> loggerMock = null!;
    private protected Mock<IFileTypeValidator> fileTypeValidator = null!;
    private protected Mock<IAuthorizationService> authorizationServiceMock = null!;
    private protected Mock<IAccountRepository> accountRepositoryMock = null!;
    private protected Mock<IStorageService> storageServiceMock = null!;
    private protected Mock<IUnitOfWork> unitOfWorkMock = null!;
    private protected Mock<ITransactionRepository> transactionRepositoryMock = null!;
    private protected Mock<IStatementRepository> statementRepositoryMock = null!;
    private protected Mock<IValidator<DebitStatementImportModel>> validatorMock = null!;
    private protected Mock<IStatementFactory> statementFactoryMock = null!;
    private protected Mock<ISpreadsheetParser> parserMock = null!;

    private protected DebitImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new Mock<ILogger<DebitImportService>>();
        fileTypeValidator = new Mock<IFileTypeValidator>();
        authorizationServiceMock = new Mock<IAuthorizationService>();
        accountRepositoryMock = new Mock<IAccountRepository>();
        storageServiceMock = new Mock<IStorageService>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        transactionRepositoryMock = new Mock<ITransactionRepository>();
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
            unitOfWorkMock.Object,
            transactionRepositoryMock.Object,
            statementRepositoryMock.Object,
            validatorMock.Object,
            statementFactoryMock.Object,
            parserMock.Object);
    }

    private protected void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private protected ISetup<IValidator<DebitStatementImportModel>, ValidationResult> SetupValidator()
        => validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<DebitStatementImportModel>>()));

    private protected void SetupStatementFactory(DebitStatementImportModel request, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(request, account)).Returns(statement);
}
