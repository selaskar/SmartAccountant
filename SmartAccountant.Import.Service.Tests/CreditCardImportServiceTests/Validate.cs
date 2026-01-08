using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Tests.CreditCardImportServiceTests;

[TestClass]
public class Validate : Base
{
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
}
