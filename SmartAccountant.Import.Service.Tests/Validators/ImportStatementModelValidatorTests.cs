using FluentValidation.TestHelper;
using Moq;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Validators;

namespace SmartAccountant.Import.Service.Tests.Validators;

[TestClass]
public class ImportStatementModelValidatorTests
{
    private ImportStatementModelValidator sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ReturnsErrorForEmptyRequestId()
    {
        // Arrange
        ImportStatementModel model = new()
        {
            RequestId = Guid.Empty,
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => new MemoryStream([])
            }
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestId).Only();
    }

    [TestMethod]
    public void ReturnsErrorForEmptyAccountId()
    {
        // Arrange
        ImportStatementModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.Empty,
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => new MemoryStream([])
            }
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountId).Only();
    }

    [TestMethod]
    public void ReturnsErrorForInvalidImportFile()
    {
        // Arrange
        ImportStatementModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "missing extension",
                ContentType = "text/plain",
                OpenReadStream = () => new MemoryStream([])
            }
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.FileExtension).Only();
    }

    [TestMethod]
    public void ReturnsErrorForNullImportFile()
    {
        // Arrange
        ImportStatementModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = null!
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File).Only();
    }

    [TestMethod]
    public void ReturnsErrorForEmptyStream()
    {
        // Arrange
        ImportStatementModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => new MemoryStream([])
            }
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.Length).Only();
    }

    [TestMethod]
    public void ReturnsErrorForTooLongStream()
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(ImportService.MaxFileSize + 1);

        ImportStatementModel model = new()
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
                //Note that generating a stream with dummy data runs faster than using a mock object.
                //new MemoryStream(Enumerable.Repeat((byte)0, (int)ImportService.MaxFileSize + 1).ToArray()),
            }
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.Length).Only();
    }

    [TestMethod]
    public void ReturnsNoErrorForStreamAtMaxLength()
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(ImportService.MaxFileSize);

        ImportStatementModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => mockStream.Object
            }
        };

        // Act
        TestValidationResult<ImportStatementModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
