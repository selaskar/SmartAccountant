using Moq;
using Moq.Language.Flow;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Factories;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.Factories.StatementFactoryTests;

[TestClass]
public class Create
{
    private Mock<ISpreadsheetParser> parserMock = null!;
    
    private StatementFactory sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        parserMock = new Mock<ISpreadsheetParser>();

        //SetupParser(debitStatement);

        sut = new(parserMock.Object);
    }

    [TestMethod]
    public void ThrowNotImplementedExceptionForUnsupportedBalanceType()
    {
        // Arrange
        var mock = new Mock<Account>();
        mock.SetupGet(x => x.NormalBalance).Returns((BalanceType)(-1));

        // Act, Assert
        Assert.ThrowsException<NotImplementedException>(() => sut.Create(null!, mock.Object));
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
    }

    private ISetup<ISpreadsheetParser> SetupParser(DebitStatement statement)
        => parserMock.Setup(p => p.ReadStatement(statement, It.IsAny<Stream>(), It.IsAny<Bank>()));
}
