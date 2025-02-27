using FluentValidation.TestHelper;
using Moq;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Validators;

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
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(1);

        CreditCardStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => mockStream.Object,
            },
            DueDate = new DateTimeOffset(DateTime.ParseExact(dueDate, "yyyy-MM-dd", provider: null), TimeSpan.Zero),
        };

        // Act
        TestValidationResult<CreditCardStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueDate.Date).Only();
    }
}
