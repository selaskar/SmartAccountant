using FluentValidation.TestHelper;
using Moq;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core;
using SmartAccountant.Import.Service.Validators;

namespace SmartAccountant.Import.Service.Tests.Validators;

[TestClass]
public class DebitStatementImportModelValidatorTests
{
    private DebitStatementImportModelValidator sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void ReturnsErrorForEmptyRequestId()
    {
        // Arrange
        DebitStatementImportModel model = new()
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
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestId).Only();
    }

    [TestMethod]
    public void ReturnsErrorForEmptyAccountId()
    {
        // Arrange
        DebitStatementImportModel model = new()
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
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountId).Only();
    }

    [TestMethod]
    public void ReturnsErrorForInvalidImportFile()
    {
        // Arrange
        DebitStatementImportModel model = new()
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
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.FileExtension).Only();
    }

    [TestMethod]
    public void ReturnsErrorForNullImportFile()
    {
        // Arrange
        DebitStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = null!
        };

        // Act
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File).Only();
    }

    [TestMethod]
    public void ReturnsErrorForEmptyStream()
    {
        // Arrange
        DebitStatementImportModel model = new()
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
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.Length).Only();
    }

    [TestMethod]
    public void ReturnsErrorForTooLongStream()
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(AbstractImportService.MaxFileSize + 1);

        DebitStatementImportModel model = new()
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
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.Length).Only();
    }

    [TestMethod]
    public void ReturnsNoErrorForStreamAtMaxLength()
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(AbstractImportService.MaxFileSize);

        DebitStatementImportModel model = new()
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
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ReturnsErrorForInvalidStartDate()
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(1);

        DebitStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => mockStream.Object,
            }
        };

        // Act
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PeriodStart.Date).Only();
    }

    [TestMethod]
    public void ReturnsErrorForInvalidEndDate()
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(1);

        DebitStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2000, 01, 02, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(2050, 01, 01, 0, 0, 0, TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => mockStream.Object,
            }
        };

        // Act
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PeriodEnd.Date).Only();
    }

    [TestMethod]
    [DataRow("2025-02-25")]
    [DataRow("2025-02-26")]
    public void ReturnsErrorForEndDateNotLaterThanStartDate(string endDate)
    {
        // Arrange
        var mockStream = new Mock<Stream>();
        mockStream.Setup(x => x.Length).Returns(1);

        DebitStatementImportModel model = new()
        {
            RequestId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            PeriodStart = new DateTimeOffset(2025, 02, 26, 0, 0, 0, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(DateTime.ParseExact(endDate, "yyyy-MM-dd", provider: null), TimeSpan.Zero),
            File = new ImportFile()
            {
                FileName = "file.txt",
                ContentType = "text/plain",
                OpenReadStream = () => mockStream.Object,
            }
        };

        // Act
        TestValidationResult<DebitStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x).Only();
    }
}
