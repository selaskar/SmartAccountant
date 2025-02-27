using Moq;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Factories;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.Factories.StatementFactoryTests;

[TestClass]
public class Create
{
    private StatementFactory sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ThrowNotImplementedExceptionForUnsupportedBalanceType()
    {
        // Arrange
        var mock = new Mock<Account>();
        mock.SetupGet(x => x.NormalBalance).Returns((BalanceType)(-1));

        // Act, Assert
        Assert.ThrowsExactly<NotImplementedException>(() => _ = sut.Create(null!, mock.Object));
    }

    [TestMethod]
    public void ThrowImportExceptionForInvalidModelTypes()
    {
        // Arrange
        SavingAccount account = new()
        {
            Id = Guid.NewGuid(),
            AccountNumber = "0",
            Currency = Currency.USD,
        };

        CreditCard account2 = new([])
        {
            CardNumber = "0",
        };

        DebitStatementImportModel model = new()
        {
            File = null!
        };

        CreditCardStatementImportModel model2 = new()
        {
            File = null!
        };

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => _ = sut.Create(model, account2));
        Assert.ThrowsExactly<ImportException>(() => _ = sut.Create(model2, account));
    }

    [TestMethod]
    public void ThrowImportExceptionForInvalidAccountTypes()
    {
        // Arrange
        var mockAccount1 = new Mock<Account>();
        mockAccount1.SetupGet(x => x.NormalBalance).Returns(BalanceType.Debit);

        var mockAccount2 = new Mock<Account>();
        mockAccount2.SetupGet(x => x.NormalBalance).Returns(BalanceType.Credit);

        DebitStatementImportModel model1 = new()
        {
            File = null!
        };

        CreditCardStatementImportModel model2 = new()
        {
            File = null!
        };

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => _ = sut.Create(model1, mockAccount1.Object));
        Assert.ThrowsExactly<ImportException>(() => _ = sut.Create(model2, mockAccount2.Object));
    }

    [TestMethod]
    public void CreateDebitStatementForDebitBalanceType()
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
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
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
    public void CreateCreditCardStatementForCreditBalanceType()
    {
        // Arrange
        CreditCard account = new([])
        {
            Id = Guid.NewGuid(),
            CardNumber = "0",
        };

        CreditCardStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = account.Id,
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = null!,
            RolloverAmount = null,
            TotalDueAmount = 1000,
            MinimumDueAmount = 300,
            TotalFees = null,
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
