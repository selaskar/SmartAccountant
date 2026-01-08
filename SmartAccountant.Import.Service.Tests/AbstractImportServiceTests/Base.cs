using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Import.Service.Tests.AbstractImportServiceTests;

public abstract class Base
{
    private protected Mock<ILogger<AbstractImportService>> loggerMock = null!;
    private Mock<IFileTypeValidator> fileTypeValidator = null!;
    private protected Mock<IAuthorizationService> authorizationServiceMock = null!;
    private protected Mock<IAccountRepository> accountRepositoryMock = null!;
    private protected Mock<IStorageService> storageServiceMock = null!;
    private protected Mock<IUnitOfWork> unitOfWorkMock = null!;
    private protected Mock<IStatementRepository> statementRepositoryMock = null!;
    private Mock<IValidator<AbstractStatementImportModel>> validatorMock = null!;
    private protected Mock<ITransactionRepository> transactionRepositoryMock = null!;
    private protected Mock<IStatementFactory> statementFactoryMock = null!;
    private Mock<IDateTimeService> dateTimeServiceMock = null!;
    private Mock<ISpreadsheetParser> parserMock = null!;

    private protected AbstractImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new();
        fileTypeValidator = new();
        authorizationServiceMock = new();
        accountRepositoryMock = new();
        storageServiceMock = new();
        unitOfWorkMock = new();
        statementRepositoryMock = new();
        validatorMock = new();
        transactionRepositoryMock = new();
        statementFactoryMock = new();
        dateTimeServiceMock = new();
        parserMock = new();

        sut = new TestImportService(
            loggerMock.Object,
            fileTypeValidator.Object,
            authorizationServiceMock.Object,
            accountRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object,
            statementRepositoryMock.Object,
            validatorMock.Object,
            transactionRepositoryMock.Object,
            statementFactoryMock.Object,
            dateTimeServiceMock.Object,
            parserMock.Object);
    }

    private protected void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private protected ISetup<IValidator<AbstractStatementImportModel>, ValidationResult> SetupValidator()
        => validatorMock.Setup(v => v.Validate(It.IsAny<AbstractStatementImportModel>()));

    private protected void SetupFileTypeValidator(bool result)
        => fileTypeValidator.Setup(f => f.IsValidFile(It.IsAny<ImportFile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

    private protected void SetupAuthorizationService(Guid userId)
        => authorizationServiceMock.SetupGet(a => a.UserId).Returns(userId);

    private protected void SetupAccountRepository(Account account)
        => accountRepositoryMock.Setup(a => a.GetAccountsOfUser(account.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new[] { account }));

    private protected void SetupStatementFactory(TestStatementImportModel model, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(model, account)).Returns(statement);

    private protected ISetup<ISpreadsheetParser> SetupParser(TestStatement statement, Bank? bank)
        => parserMock.Setup(p => p.ReadStatement(statement, It.IsAny<Stream>(), bank ?? It.IsAny<Bank>()));

    private protected ISetup<IStorageService, Task> SetupStorageService()
        => storageServiceMock.Setup(s => s.WriteToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()));

    private protected ISetup<ITransactionRepository, Task<Transaction[]>> SetupTransactionRepository(Guid accountId)
        => transactionRepositoryMock.Setup(s => s.GetTransactionsOfAccount(accountId, It.IsAny<CancellationToken>()));

    private protected ISetup<IStatementRepository, Task> SetupStatementRepository(TestStatement statement)
        => statementRepositoryMock.Setup(s => s.Insert(statement, It.IsAny<CancellationToken>()));

    internal sealed class TestImportService(
        ILogger<AbstractImportService> logger,
        IFileTypeValidator fileTypeValidator,
        IAuthorizationService authorizationService,
        IAccountRepository accountRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork,
        IStatementRepository statementRepository,
        IValidator<AbstractStatementImportModel> validator,
        ITransactionRepository transactionRepository,
        IStatementFactory statementFactory,
        IDateTimeService dateTimeService,
        ISpreadsheetParser parser)
        : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService)
    {
        protected internal override void Validate(AbstractStatementImportModel model)
            => validator.Validate(model);

        protected internal override Task<Statement> Parse(AbstractStatementImportModel model, Account account, CancellationToken _)
        {
            var statement = (TestStatement)statementFactory.Create(model, account);

            parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);

            return Task.FromResult<Statement>(statement);
        }

        protected internal override Transaction[] DetectNew(Statement statement, Transaction[] existingTransactions)
        {
            return [];
        }

        protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
        {
            return [];
        }
    }

    internal sealed record class TestStatementImportModel : AbstractStatementImportModel { }

    internal sealed record class TestStatement : Statement<TestTransaction> { }

    internal sealed record class TestTransaction : Transaction { }
}
