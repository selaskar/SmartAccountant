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

namespace SmartAccountant.Import.Service.Tests.ImportServiceTests;

[TestClass]
public class ImportStatementTests
{
    private Mock<ILogger<ImportService>> loggerMock = null!;
    private Mock<IValidator<ImportStatementModel>> validatorMock = null!;
    private Mock<IFileTypeValidator> fileTypeValidator = null!;
    private Mock<IAuthorizationService> authorizationServiceMock = null!;
    private Mock<IAccountRepository> accountRepositoryMock = null!;
    private Mock<IStatementFactory> statementFactoryMock = null!;
    private Mock<ISpreadsheetParser> parserMock = null!;
    private Mock<IStorageService> storageServiceMock = null!;
    private Mock<IStatementRepository> statementRepositoryMock = null!;

    private ImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new Mock<ILogger<ImportService>>();
        validatorMock = new Mock<IValidator<ImportStatementModel>>();
        fileTypeValidator = new Mock<IFileTypeValidator>();
        authorizationServiceMock = new Mock<IAuthorizationService>();
        accountRepositoryMock = new Mock<IAccountRepository>();
        statementFactoryMock = new Mock<IStatementFactory>();
        parserMock = new Mock<ISpreadsheetParser>();
        storageServiceMock = new Mock<IStorageService>();
        statementRepositoryMock = new Mock<IStatementRepository>();

        sut = new ImportService(
            loggerMock.Object,
            validatorMock.Object,
            fileTypeValidator.Object,
            authorizationServiceMock.Object,
            accountRepositoryMock.Object,
            statementFactoryMock.Object,
            parserMock.Object,
            storageServiceMock.Object,
            statementRepositoryMock.Object);
    }

    [TestMethod]
    public async Task ThrowValidationExceptionForInvalidModel()
    {
        // Arrange
        ImportStatementModel model = new()
        {
            File = null!,
        };

        SetupValidator().Throws(new ValidationException("Validation failed"));

        // Act, Assert
        await Assert.ThrowsExceptionAsync<ValidationException>(() => sut.ImportStatement(model, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForInvalidFileType()
    {
        // Arrange
        var request = new ImportStatementModel()
        {
            File = null!
        };

        SetupValidator();

        SetupFileTypeValidator(false);

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        Assert.AreEqual(Messages.UploadedStatementFileTypeNotSupported, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnauthenticatedUser()
    {
        // Arrange
        var request = new ImportStatementModel()
        {
            File = null!
        };

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(null);

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        Assert.AreEqual(Messages.UserNotAuthenticated, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForInvalidAccount()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
        {
            AccountId = accountId,
            File = null!
        };

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId)).Returns(AsyncEnumerable.Empty<Account>());

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        Assert.AreEqual(Messages.AccountDoesNotBelongToUser, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInAccountRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
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
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        Assert.AreEqual(Messages.CannotValidateAccountHolder, result.Message);
    }

    [TestMethod]
    public async Task ThrowOperationCancelledExceptionForCancelledOperationInAccountRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
        {
            AccountId = accountId,
            File = null!
        };

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId))
            .Throws<OperationCanceledException>();

        // Act, Assert
        await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => sut.ImportStatement(request, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementFactory()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
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

        statementFactoryMock.Setup(s => s.Create(request, account)).Throws<NotImplementedException>();

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInParserService()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
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

        DebitStatement debitStatement = new();

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(request, account, debitStatement);

        SetupParser(debitStatement).Throws<InvalidOperationException>();

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStorageService()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
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

        DebitStatement debitStatement = new();

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(request, account, debitStatement);

        SetupParser(debitStatement);

        SetupStorageService().Throws(new StorageException("test"));

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotSaveUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementRepository()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
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

        DebitStatement debitStatement = new();

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(request, account, debitStatement);

        SetupParser(debitStatement);

        SetupStorageService();

        SetupStatementRepository(debitStatement).Throws<InvalidOperationException>();

        // Act, Assert
        var result = await Assert.ThrowsExceptionAsync<ImportException>(() => sut.ImportStatement(request, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(1, debitStatement.Documents.Count);

        Assert.AreEqual(Messages.CannotSaveImportedStatement, result.Message);
    }

    [TestMethod]
    public async Task Succeed()
    {
        // Arrange
        var accountId = Guid.Parse("91DF13C1-7261-400B-9413-D7DA42B392D0");

        var request = new ImportStatementModel()
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

        DebitStatement debitStatement = new();

        SetupLogger(LogLevel.Trace, true);

        SetupValidator();

        SetupFileTypeValidator(true);

        SetupAuthorizationService(accountId);

        SetupAccountRepository(account);

        SetupStatementFactory(request, account, debitStatement);

        SetupParser(debitStatement);

        SetupStorageService();

        SetupStatementRepository(debitStatement);

        // Act
        Statement result = await sut.ImportStatement(request, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Trace), Times.AtLeastOnce);

        storageServiceMock.Verify(s => s.WriteToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.AreEqual(1, debitStatement.Documents.Count);

        statementRepositoryMock.Verify(s => s.Insert(debitStatement, It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private ISetup<IValidator<ImportStatementModel>, ValidationResult> SetupValidator()
        => validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<ImportStatementModel>>()));

    private void SetupFileTypeValidator(bool result)
        => fileTypeValidator.Setup(f => f.IsValidFile(It.IsAny<ImportFile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

    private void SetupAuthorizationService(Guid? userId)
        => authorizationServiceMock.SetupGet(a => a.UserId).Returns(userId);

    private void SetupAccountRepository(Account account)
        => accountRepositoryMock.Setup(a => a.GetAccountsOfUser(account.Id))
            .Returns(AsyncEnumerable.ToAsyncEnumerable([account]));

    private void SetupStatementFactory(ImportStatementModel request, Account account, Statement statement)
        => statementFactoryMock.Setup(s => s.Create(request, account)).Returns(statement);

    private ISetup<ISpreadsheetParser> SetupParser(DebitStatement statement)
        => parserMock.Setup(p => p.ReadStatement(statement, It.IsAny<Stream>()));

    private ISetup<IStorageService, Task> SetupStorageService()
        => storageServiceMock.Setup(s => s.WriteToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()));

    private ISetup<IStatementRepository, Task> SetupStatementRepository(DebitStatement statement)
        => statementRepositoryMock.Setup(s => s.Insert(statement, It.IsAny<CancellationToken>()));
}
