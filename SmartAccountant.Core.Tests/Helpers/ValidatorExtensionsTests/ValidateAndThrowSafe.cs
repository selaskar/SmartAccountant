using FluentValidation;
using SmartAccountant.Core.Helpers;

namespace SmartAccountant.Core.Tests.Helpers.ValidatorExtensionsTests;

[TestClass]
public sealed class ValidateAndThrowSafe
{
    [TestMethod]
    public void ThrowArgumentNullExceptionForNullModel()
    {
        // Arrange
        var validator = new InlineValidator<object>();

        // Act, Assert
        Assert.ThrowsException<ArgumentNullException>(() => ValidatorExtensions.ValidateAndThrowSafe(validator, null!));
    }

    [TestMethod]
    public void ThrowValidationExceptionForInvalidModel()
    {
        // Arrange
        var validator = new InlineValidator<bool>();
        validator.RuleFor(x => x).Equal(true);

        // Act, Assert
        Assert.ThrowsException<ValidationException>(() => ValidatorExtensions.ValidateAndThrowSafe(validator, false));
    }

    [TestMethod]
    public void NotThrowValidationExceptionForValidModel()
    {
        // Arrange
        var validator = new InlineValidator<bool>();
        validator.RuleFor(x => x).Equal(true);

        // Act
        ValidatorExtensions.ValidateAndThrowSafe(validator, true);
    }
}
