using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service.Tests.AbstractImportServiceTests;

[TestClass]
public class ImportStatement
{
    private Mock<ILogger<AbstractImportService>> loggerMock = null!;
    private Mock<IFileTypeValidator> fileTypeValidator = null!;
    private Mock<IAuthorizationService> authorizationServiceMock = null!;
    private Mock<IAccountRepository> accountRepositoryMock = null!;
    private Mock<IStorageService> storageServiceMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IStatementRepository> statementRepositoryMock = null!;
    private Mock<IValidator<AbstractStatementImportModel>> validatorMock = null!;
    private Mock<ITransactionRepository> transactionRepositoryMock = null!;
    private Mock<IStatementFactory> statementFactoryMock = null!;
    private Mock<ISpreadsheetParser> parserMock = null!;

    private AbstractImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new Mock<ILogger<AbstractImportService>>();
        fileTypeValidator = new Mock<IFileTypeValidator>();
        authorizationServiceMock = new Mock<IAuthorizationService>();
        accountRepositoryMock = new Mock<IAccountRepository>();
        storageServiceMock = new Mock<IStorageService>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        statementRepositoryMock = new Mock<IStatementRepository>();
        validatorMock = new Mock<IValidator<AbstractStatementImportModel>>();
        transactionRepositoryMock = new Mock<ITransactionRepository>();
        statementFactoryMock = new Mock<IStatementFactory>();
        parserMock = new Mock<ISpreadsheetParser>();

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
            parserMock.Object);
    }

    [TestMethod]
    public async Task ThrowValidationExceptionForInvalidModel()
    {
        // Arrange
        var model = new Mock<AbstractStatementImportModel>().Object;

        SetupValidator().Throws(new ValidationException("Validation failed"));

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ValidationException>(() => sut.ImportStatement(model, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForInvalidFileType()
    {
        // Arrange
        var model = new Mock<AbstractStatementImportModel>().Object;

        SetupValidator();

        SetupFileTypeValidator(false);

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        Assert.AreEqual(Messages.UploadedStatementFileTypeNotSupported, result.Message);
    }

    [TestMethod]
    public async Task ThrowAuthenticationExceptionForUnauthenticatedUser()
    {
        // Arrange
        var model = new Mock<AbstractStatementImportModel>().Object;

        SetupValidator();

        SetupFileTypeValidator(true);

        authorizationServiceMock.SetupGet(a => a.UserId).Throws(new AuthenticationException("test"));

        // Act, Assert
        await Assert.ThrowsExactlyAsync<AuthenticationException>(() => sut.ImportStatement(model, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForInvalidAccount()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        TestStatementImportModel model = new()
        {
            AccountId = accountId,
            File = null!,
        };

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId)).Returns(AsyncEnumerable.Empty<Account>());

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        Assert.AreEqual(Messages.AccountDoesNotBelongToUser, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInAccountRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = null!
        };

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId))
            .Throws(new RepositoryException("test", null!));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotValidateAccountHolder, result.Message);
    }

    [TestMethod]
    public async Task ThrowOperationCancelledExceptionForCancelledOperationInAccountRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = null!
        };

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId))
            .Throws<TaskCanceledException>();

        // Act, Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.ImportStatement(model, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForImportExceptionInStatementFactory()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = null!
        };

        var account = new SavingAccount()
        {
            Id = accountId,
            AccountNumber = "0"
        };

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        statementFactoryMock.Setup(s => s.Create(model, account)).Throws(new ImportException("test"));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForImportExceptionInParserService()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        var account = new SavingAccount()
        {
            Id = accountId,
            AccountNumber = "0"
        };

        TestStatement testStatement = new();

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(model, account, testStatement);

        SetupParser(testStatement, bank: null).Throws(new ImportException("test"));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStorageService()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        var account = new SavingAccount()
        {
            Id = accountId,
            AccountNumber = "0"
        };

        TestStatement testStatement = new();

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(model, account, testStatement);

        SetupParser(testStatement, bank: null);

        SetupStorageService().Throws(new StorageException("test"));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotSaveUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        var account = new SavingAccount()
        {
            Id = accountId,
            AccountNumber = "0"
        };

        TestStatement testStatement = new();

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(model, account, testStatement);

        SetupParser(testStatement, bank: null);

        SetupStorageService();

        SetupStatementRepository(testStatement).Throws<InvalidOperationException>();

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(1, testStatement.Documents.Count);

        Assert.AreEqual(Messages.CannotSaveImportedStatement, result.Message);

        unitOfWorkMock.Verify(s => s.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForRepositoryExceptionInTransactionRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        var account = new SavingAccount()
        {
            Id = accountId,
            AccountNumber = "0"
        };

        TestStatement testStatement = new();

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(model, account, testStatement);

        SetupParser(testStatement, bank: null);

        SetupStorageService();

        SetupStatementRepository(testStatement);

        SetupTransactionRepository(accountId).Throws(new RepositoryException("test", null!));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        Assert.AreEqual(1, testStatement.Documents.Count);

        Assert.Contains(accountId.ToString(), result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [TestMethod]
    public async Task Succeed()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var model = new TestStatementImportModel()
        {
            AccountId = accountId,
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => new MemoryStream()
            }
        };

        var account = new SavingAccount()
        {
            Id = accountId,
            AccountNumber = "0"
        };

        TestStatement testStatement = new();

        SetupLogger(LogLevel.Trace, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(model, account, testStatement);

        SetupParser(testStatement, bank: null);

        SetupStorageService();

        SetupStatementRepository(testStatement);

        SetupTransactionRepository(accountId);

        // Act
        Statement result = await sut.ImportStatement(model, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Trace), Times.AtLeastOnce);

        storageServiceMock.Verify(s => s.WriteToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.AreEqual(1, testStatement.Documents.Count);

        statementRepositoryMock.Verify(s => s.Insert(testStatement, It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(s => s.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    private void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private ISetup<IValidator<AbstractStatementImportModel>, ValidationResult> SetupValidator()
        => validatorMock.Setup(v => v.Validate(It.IsAny<AbstractStatementImportModel>()));

    private void SetupFileTypeValidator(bool result)
        => fileTypeValidator.Setup(f => f.IsValidFile(It.IsAny<ImportFile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

    private void SetupAuthorizationService(Guid userId)
        => authorizationServiceMock.SetupGet(a => a.UserId).Returns(userId);

    private void SetupAccountRepository(Account account)
        => accountRepositoryMock.Setup(a => a.GetAccountsOfUser(account.Id))
            .Returns(AsyncEnumerable.ToAsyncEnumerable([account]));

    private void SetupStatementFactory(TestStatementImportModel model, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(model, account)).Returns(statement);

    private ISetup<ISpreadsheetParser> SetupParser(TestStatement statement, Bank? bank)
        => parserMock.Setup(p => p.ReadStatement(statement, It.IsAny<Stream>(), bank ?? It.IsAny<Bank>()));

    private ISetup<IStorageService, Task> SetupStorageService()
        => storageServiceMock.Setup(s => s.WriteToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()));

    private ISetup<ITransactionRepository, Task<Transaction[]>> SetupTransactionRepository(Guid accountId)
        => transactionRepositoryMock.Setup(s => s.GetTransactionsOfAccount(accountId, It.IsAny<CancellationToken>()));

    private ISetup<IStatementRepository, Task> SetupStatementRepository(TestStatement statement)
        => statementRepositoryMock.Setup(s => s.Insert(statement, It.IsAny<CancellationToken>()));


    private sealed class TestImportService(
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
        ISpreadsheetParser parser)
        : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository)
    {
        protected internal override void Validate(AbstractStatementImportModel model)
            => validator.Validate(model);

        protected internal override Statement Parse(AbstractStatementImportModel model, Account account)
        {
            var statement = (TestStatement)statementFactory.Create(model, account);

            parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);

            return statement;
        }

        protected internal override Transaction[] DetectExisting(Statement statement, Transaction[] existingTransactions)
        {
            return [];
        }

        protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
        {
            return [];
        }
    }

    private sealed class TestStatementImportModel : AbstractStatementImportModel { }

    private sealed record class TestStatement : Statement<TestTransaction> { }

    private sealed record class TestTransaction : Transaction { }
}
