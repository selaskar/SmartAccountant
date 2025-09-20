using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.MultipartCreditCardImportServiceTests;

[TestClass]
public class Parse : Base
{
    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedErrorInStatementFactory()
    {
        // Arrange
        MultipartStatementImportModel model = new()
        {
            File = null!
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws<NotImplementedException>();

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task DirectlyThrowImportExceptionInStatementFactory()
    {
        // Arrange
        MultipartStatementImportModel model = new()
        {
            File = null!
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        statementFactoryMock.Setup(s => s.Create(model, account))
            .Throws(new ImportException("test"));

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual("test", result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForParserExceptionInParser()
    {
        // Arrange
        MultipartStatementImportModel model = new()
        {
            File = null! //throws the exception
        };

        CreditCard account = new()
        {
            CardNumber = "0"
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, null!);

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Once);

        Assert.AreEqual(Messages.CannotParseUploadedStatementFile, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUnexpectedPrimaryAccountType()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        SavingAccount account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
            AccountNumber = "0",
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUndeterminedCardNumber1()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = null;
                statement.CardNumber2 = "0";
            });

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        Assert.AreEqual(Messages.PrimaryCardNumberNotDetermined, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForUndeterminedCardNumber2()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = "0";
                statement.CardNumber2 = null;
            });

        // Act, Assert
        var result = await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));

        Assert.AreEqual(Messages.SecondaryCardNumberNotDetermined, result.Message);
    }

    [TestMethod]
    public async Task ThrowImportExceptionForDiscoveredCardNumberMismatch()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = "1";
                statement.CardNumber2 = "2";
            });

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowOperationCancelledExceptionForCancelledToken()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        VirtualCard secondAccount = new()
        {
            Id = Guid.Parse("1500D49D-A4CF-4957-B33D-3AADB1039F8D"),
            CardNumber = "1",
        };

        CreditCard unrelatedAccount = new()
        {
            CardNumber = "2",
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = account.CardNumber;
                statement.CardNumber2 = secondAccount.CardNumber;
            });

        SetupAccountRepository(account.HolderId, account, unrelatedAccount, secondAccount);

        using CancellationTokenSource cts = new();
        await cts.CancelAsync();

        // Act, Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await sut.Parse(model, account, cts.Token));
    }

    [TestMethod]
    public async Task FailForMissingSecondAccount()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = account.CardNumber;
                statement.CardNumber2 = "1";
            });

        SetupAccountRepository(account.HolderId, account);

        // Act, Assert
        await Assert.ThrowsExactlyAsync<ImportException>(async () => await sut.Parse(model, account, CancellationToken.None));
    }

    [TestMethod]
    public async Task Succeed()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        VirtualCard secondAccount = new()
        {
            Id = Guid.Parse("1500D49D-A4CF-4957-B33D-3AADB1039F8D"),
            CardNumber = "1",
        };

        SavingAccount unrelatedAccount = new()
        {
            AccountNumber = "2",
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
            Transactions = [new CreditCardTransaction()],
            SecondaryTransactions = [new CreditCardTransaction()],
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = account.CardNumber;
                statement.CardNumber2 = secondAccount.CardNumber;
            });

        SetupAccountRepository(account.HolderId, account, unrelatedAccount, secondAccount);

        // Act
        Statement result = await sut.Parse(model, account, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);

        Assert.AreEqual(secondAccount.Id, statement.DependentAccountId);

        Assert.AreEqual(account.Id, statement.Transactions[0].AccountId);
        Assert.AreEqual(secondAccount.Id, statement.SecondaryTransactions[0].AccountId);
    }

    [TestMethod]
    public async Task SucceedInReverseOrder()
    {
        // Arrange
        Stream stream = new MemoryStream();
        MultipartStatementImportModel model = new()
        {
            File = new ImportFile()
            {
                ContentType = "",
                FileName = "test",
                OpenReadStream = () => stream,
            }
        };

        CreditCard account = new()
        {
            Id = Guid.Parse("608AAE8B-2059-4B4F-978F-D63D3EF0B704"),
            CardNumber = "0",
            HolderId = Guid.Parse("C11C3EBF-B848-4A09-8A74-F42981C1C4CD"),
        };

        VirtualCard secondAccount = new()
        {
            Id = Guid.Parse("1500D49D-A4CF-4957-B33D-3AADB1039F8D"),
            CardNumber = "1"
        };

        VirtualCard unrelatedAccount = new()
        {
            CardNumber = "2",
        };

        SharedStatement statement = new()
        {
            AccountId = account.Id,
            Transactions = [new CreditCardTransaction()],
            SecondaryTransactions = [new CreditCardTransaction()],
        };

        SetupLogger(LogLevel.Error, true);

        SetupStatementFactory(model, account, statement);

        parserMock.Setup(x => x.ReadMultipartStatement(statement, stream, It.IsAny<Bank>()))
            .Callback<SharedStatement, Stream, Bank>((statement, _, _) =>
            {
                statement.CardNumber1 = secondAccount.CardNumber;
                statement.CardNumber2 = account.CardNumber;
            });

        SetupAccountRepository(account.HolderId, account, unrelatedAccount, secondAccount);

        // Act
        Statement result = await sut.Parse(model, account, CancellationToken.None);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);

        Assert.AreEqual(statement, result);

        Assert.AreEqual(secondAccount.Id, statement.DependentAccountId);

        Assert.AreEqual(secondAccount.Id, statement.Transactions[0].AccountId);
        Assert.AreEqual(account.Id, statement.SecondaryTransactions[0].AccountId);
    }
}
