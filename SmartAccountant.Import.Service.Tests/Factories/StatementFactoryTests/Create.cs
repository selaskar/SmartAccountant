using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Import.Service.Factories;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Import.Service.Tests.Factories.StatementFactoryTests;

[TestClass]
public class Create
{
    private StatementFactory sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ThrowNotImplementedExceptionForImportModelType()
    {
        // Arrange
        Mock<AbstractStatementImportModel> mock = new();

        // Act, Assert
        Assert.ThrowsExactly<NotImplementedException>(() => _ = sut.Create(mock.Object, null!));
    }

    [TestMethod]
    public void ThrowImportExceptionForInvalidModelTypes()
    {
        // Arrange
        CreditCard account = new()
        {
            CardNumber = "0",
        };

        DebitStatementImportModel model = new()
        {
            File = null!
        };

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => _ = sut.Create(model, account));
    }

    [TestMethod]
    public void CreateDebitStatementForDebitImportModel()
    {
        // Arrange
        SavingAccount account = new()
        {
            Id = Guid.NewGuid(),
            AccountNumber = "0",
            Currency = Currency.USD,
        };

        DebitStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = account.Id,
            File = null!
        };

        // Act
        Statement? result = sut.Create(model, account);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<DebitStatement>(result);
        Assert.AreEqual(account.Id, result.AccountId);
    }

    [TestMethod]
    public void CreateSharedStatementForMultipartImportType()
    {
        // Arrange
        CreditCard account = new()
        {
            Id = Guid.NewGuid(),
            CardNumber = "0",
        };

        MultipartStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = account.Id,
            File = null!,
            RolloverAmount = 0,
            TotalDueAmount = 1000,
            MinimumDueAmount = 300,
            TotalFees = 0,
            DueDate = new DateTimeOffset(2025, 03, 06, 0, 0, 0, TimeSpan.Zero),
        };

        // Act
        Statement? result = sut.Create(model, account);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SharedStatement>(result);
        Assert.AreEqual(account.Id, result.AccountId);
    }

    [TestMethod]
    public void CreateCreditCardStatementForCreditCardImportType()
    {
        // Arrange
        CreditCard account = new()
        {
            Id = Guid.NewGuid(),
            CardNumber = "0",
        };

        CreditCardStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = account.Id,
            File = null!,
            RolloverAmount = 0,
            TotalDueAmount = 1000,
            MinimumDueAmount = 300,
            TotalFees = 0,
            DueDate = new DateTimeOffset(2025, 03, 06, 0, 0, 0, TimeSpan.Zero),
        };

        // Act
        Statement? result = sut.Create(model, account);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<CreditCardStatement>(result);
        Assert.AreEqual(account.Id, result.AccountId);
    }
}
