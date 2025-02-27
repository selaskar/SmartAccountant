using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service.Tests.CreditCardImportServiceTests;

[TestClass]
public class Validate
{
    private Mock<ILogger<CreditCardImportService>> loggerMock = null!;
    private Mock<IFileTypeValidator> fileTypeValidator = null!;
    private Mock<IAuthorizationService> authorizationServiceMock = null!;
    private Mock<IAccountRepository> accountRepositoryMock = null!;
    private Mock<IStorageService> storageServiceMock = null!;
    private Mock<IStatementRepository> statementRepositoryMock = null!;
    private Mock<IValidator<CreditCardStatementImportModel>> validatorMock = null!;
    private Mock<IStatementFactory> statementFactoryMock = null!;
    private Mock<ISpreadsheetParser> parserMock = null!;

    private CreditCardImportService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        loggerMock = new Mock<ILogger<CreditCardImportService>>();
        fileTypeValidator = new Mock<IFileTypeValidator>();
        authorizationServiceMock = new Mock<IAuthorizationService>();
        accountRepositoryMock = new Mock<IAccountRepository>();
        storageServiceMock = new Mock<IStorageService>();
        statementRepositoryMock = new Mock<IStatementRepository>();
        validatorMock = new Mock<IValidator<CreditCardStatementImportModel>>();
        statementFactoryMock = new Mock<IStatementFactory>();
        parserMock = new Mock<ISpreadsheetParser>();

        sut = new(
            loggerMock.Object,
            fileTypeValidator.Object,
            authorizationServiceMock.Object,
            accountRepositoryMock.Object,
            storageServiceMock.Object,
            statementRepositoryMock.Object,
            validatorMock.Object,
            statementFactoryMock.Object,
            parserMock.Object);
    }

    [TestMethod]
    public void ThrowValidationExceptionForInvalidModel()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = null!
        };

        SetupValidator().Throws(new ValidationException("Validation failed"));

        // Act, Assert
        Assert.ThrowsExactly<ValidationException>(() => sut.Validate(model));
    }

    [TestMethod]
    public void Succeed()
    {
        // Arrange
        CreditCardStatementImportModel model = new()
        {
            File = null!
        };

        SetupLogger(LogLevel.Error, true);
        
        SetupValidator();

        // Act
        sut.Validate(model);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);
    }


    private void SetupLogger(LogLevel logLevel, bool enabled)
        => loggerMock.Setup(l => l.IsEnabled(logLevel)).Returns(enabled);

    private ISetup<IValidator<CreditCardStatementImportModel>, ValidationResult> SetupValidator()
        => validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<CreditCardStatementImportModel>>()));
}
