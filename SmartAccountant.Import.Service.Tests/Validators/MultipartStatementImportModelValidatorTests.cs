using FluentValidation.TestHelper;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Validators;

namespace SmartAccountant.Import.Service.Tests.Validators;

[TestClass]
public class MultipartStatementImportModelValidatorTests
{
    private MultipartStatementImportModelValidator sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new();

    [TestMethod]
    public void FailForBaseError()
    {
        // Arrange
        MultipartStatementImportModel model = new()
        {
            File = null!,
        };

        // Act
        TestValidationResult<MultipartStatementImportModel> result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestId);
    }
}
