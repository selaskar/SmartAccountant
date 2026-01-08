using FluentValidation.TestHelper;
using SmartAccountant.Import.Service.Validators;
using SmartAccountant.Models.Request;

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
