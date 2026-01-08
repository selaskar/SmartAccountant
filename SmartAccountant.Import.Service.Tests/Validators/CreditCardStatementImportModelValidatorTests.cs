using FluentValidation.TestHelper;
using Moq;
using SmartAccountant.Import.Service.Validators;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Tests.Validators;

[TestClass]
public class CreditCardStatementImportModelValidatorTests
{
    private CreditCardStatementImportModelValidator sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();


    [TestMethod]
    [DataRow("2000-01-01")]
    [DataRow("2050-01-01")]
    public void ReturnsErrorForInvalidDueDate(string dueDate)
    {
        // Arrange
        CreditCardStatementImportModel model = GetBaseModel() with
        {
            DueDate = new DateTimeOffset(DateTime.ParseExact(dueDate, "yyyy-MM-dd", provider: null), TimeSpan.Zero),
        };

        // Act
        TestValidationResult<CreditCardStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueDate.Date).Only();
    }

    [TestMethod]
    public void ReturnsErrorForInconsistentDueAmount()
    {
        // Arrange
        CreditCardStatementImportModel model = GetBaseModel() with
        {
            DueDate = new DateTimeOffset(new DateTime(2025, 09, 17), TimeSpan.Zero),
            RolloverAmount = 100,
            TotalExpenses = 1000,
            TotalFees = 10,
            TotalPayments = 1,
            TotalDueAmount = 1111,
        };

        // Act
        TestValidationResult<CreditCardStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x).Only();
    }

    [TestMethod]
    public void ReturnsNoErrorForInconsistentDueAmount()
    {
        // Arrange
        CreditCardStatementImportModel model = GetBaseModel() with
        {
            DueDate = new DateTimeOffset(new DateTime(2025, 09, 17), TimeSpan.Zero),
            RolloverAmount = 100,
            TotalExpenses = 1000,
            TotalFees = 10,
            TotalPayments = 1,
            TotalDueAmount = 1109,
        };

        // Act
        TestValidationResult<CreditCardStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }


    private static CreditCardStatementImportModel GetBaseModel()
    {
        Mock<Stream> mockStream = new();
        mockStream.Setup(x => x.Length).Returns(1);

        CreditCardStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => mockStream.Object,
            },
        };

        return model;
    }
}
