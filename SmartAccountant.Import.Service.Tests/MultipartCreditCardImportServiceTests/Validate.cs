using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Tests.MultipartCreditCardImportServiceTests;

[TestClass]
public class Validate : Base
{
    [TestMethod]
    public void ThrowValidationExceptionForInvalidModel()
    {
        // Arrange
        MultipartStatementImportModel model = new()
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
        MultipartStatementImportModel model = new()
        {
            File = null!,
            RolloverAmount = 1,
            TotalExpenses = 2,
            TotalFees = 3,
            TotalPayments = 0,
            TotalDueAmount = 6,
        };

        SetupLogger(LogLevel.Error, true);

        SetupValidator();

        // Act
        sut.Validate(model);

        // Assert
        loggerMock.Verify(l => l.IsEnabled(LogLevel.Error), Times.Never);
    }
}
