using FluentValidation.TestHelper;
using SmartAccountant.Import.Service.Validators;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Tests.Validators;

[TestClass]
public class ImportFileValidatorTests
{
    private ImportFileValidator sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void ReturnsErrorForEmptyFileName(string fileName)
    {
        // Arrange
        ImportFile model = new()
        {
            FileName = fileName,
            ContentType = "text/plain",
            OpenReadStream = () => new MemoryStream([]),
        };

        // Act
        TestValidationResult<ImportFile> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FileName);
        result.ShouldHaveValidationErrorFor(x => x.FileExtension);
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("a.")]
    [DataRow("a. ")]
    [DataRow(".")]
    [DataRow("..")]
    [DataRow("...")]
    public void ReturnsErrorForEmptyExtension(string fileName)
    {
        // Arrange
        ImportFile model = new()
        {
            FileName = fileName,
            ContentType = "text/plain",
            OpenReadStream = () => new MemoryStream([]),
        };

        // Act
        TestValidationResult<ImportFile> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FileExtension).Only();
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void ReturnsErrorForEmptyContentType(string contentType)
    {
        // Arrange
        ImportFile model = new()
        {
            FileName = "test.txt",
            ContentType = contentType,
            OpenReadStream = () => new MemoryStream([]),
        };

        // Act
        TestValidationResult<ImportFile> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContentType).Only();
    }

    [TestMethod]
    public void ReturnsErrorForNullStream()
    {
        // Arrange
        ImportFile model = new()
        {
            FileName = "test.txt",
            ContentType = "text/plain",
            OpenReadStream = null!,
        };

        // Act
        TestValidationResult<ImportFile> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OpenReadStream).Only();
    }

    [TestMethod]
    public void ReturnsNoErrorForValidModel()
    {
        // Arrange
        ImportFile model = new()
        {
            FileName = "test.txt",
            ContentType = "text/plain",
            OpenReadStream = () => new MemoryStream([]),
        };

        // Act
        TestValidationResult<ImportFile> result = sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
