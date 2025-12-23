using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.AbstractImportServiceTests;

[TestClass]
public class ImportStatement : Base
{
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

        Assert.AreEqual(ImportErrors.UploadedStatementFileTypeNotSupported, result.Error);
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

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId, It.IsAny<CancellationToken>())).Returns(Task.FromResult(Array.Empty<Account>()));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        Assert.AreEqual(ImportErrors.AccountDoesNotBelongToUser, result.Error);
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

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId, It.IsAny<CancellationToken>()))
            .Throws(new RepositoryException("test", null!));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(() => sut.ImportStatement(model, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(ImportErrors.CannotValidateAccountHolder, result.Error);
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

        accountRepositoryMock.Setup(a => a.GetAccountsOfUser(accountId, It.IsAny<CancellationToken>()))
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

        statementFactoryMock.Setup(s => s.Create(model, account)).Throws(new ImportException(ImportErrors.Unspecified, "test"));

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

        SetupParser(testStatement, bank: null).Throws(new ImportException(ImportErrors.Unspecified, "test"));

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

        Assert.AreEqual(ImportErrors.CannotSaveUploadedStatementFile, result.Error);
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

        Assert.HasCount(1, testStatement.Documents);

        Assert.AreEqual(ImportErrors.CannotSaveImportedStatement, result.Error);

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

        TestStatement testStatement = new()
        {
            AccountId = accountId,
        };

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

        Assert.HasCount(1, testStatement.Documents);

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

        Assert.HasCount(1, testStatement.Documents);

        statementRepositoryMock.Verify(s => s.Insert(testStatement, It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(s => s.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
