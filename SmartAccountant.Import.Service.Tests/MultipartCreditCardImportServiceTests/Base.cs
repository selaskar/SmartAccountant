using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service.Tests.MultipartCreditCardImportServiceTests;

public abstract class Base
{
    private protected Mock<ILogger<MultipartCreditCardImportService>> loggerMock = null!;
    private protected Mock<IFileTypeValidator> fileTypeValidator = null!;
    private protected Mock<IAuthorizationService> authorizationServiceMock = null!;
    private protected Mock<IAccountRepository> accountRepositoryMock = null!;
    private protected Mock<IStorageService> storageServiceMock = null!;
    private protected Mock<IUnitOfWork> unitOfWorkMock = null!;
    private protected Mock<ITransactionRepository> transactionRepositoryMock = null!;
    private protected Mock<IStatementRepository> statementRepositoryMock = null!;
    private protected Mock<IDateTimeService> dateTimeServiceMock = null!;
    private protected Mock<IValidator<MultipartStatementImportModel>> validatorMock = null!;
    private protected Mock<IStatementFactory> statementFactoryMock = null!;
    private protected Mock<IMultipartStatementParser> parserMock = null!;

    private protected MultipartCreditCardImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new();
        fileTypeValidator = new();
        authorizationServiceMock = new();
        accountRepositoryMock = new();
        storageServiceMock = new();
        unitOfWorkMock = new();
        transactionRepositoryMock = new();
        statementRepositoryMock = new();
        dateTimeServiceMock = new();
        validatorMock = new();
        statementFactoryMock = new();
        parserMock = new();

        sut = new(
            loggerMock.Object,
            fileTypeValidator.Object,
            authorizationServiceMock.Object,
            accountRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object,
            transactionRepositoryMock.Object,
            statementRepositoryMock.Object,
            dateTimeServiceMock.Object,
            validatorMock.Object,
            statementFactoryMock.Object,
            parserMock.Object);
    }


    private protected void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private protected void SetupAccountRepository(Guid userId, params Account[] accounts)
        => accountRepositoryMock.Setup(a => a.GetAccountsOfUser(userId, It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult(accounts));

    private protected ISetup<ITransactionRepository, Task<Transaction[]>> SetupTransactionRepository(Guid accountId)
        => transactionRepositoryMock.Setup(s => s.GetTransactionsOfAccount(accountId, It.IsAny<CancellationToken>()));

    private protected ISetup<IValidator<MultipartStatementImportModel>, ValidationResult> SetupValidator()
        => validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<MultipartStatementImportModel>>()));

    private protected void SetupStatementFactory(MultipartStatementImportModel request, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(request, account)).Returns(statement);
}
