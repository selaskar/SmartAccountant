using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace SmartAccountant.Core.Helpers;

public static class ValidatorExtensions
{
    /// <summary>
    /// Adds null-check to <see cref="IValidator{T}.Validate(T)"/> method.
    /// </summary>
    /// <exception cref="ValidationException"/>
    /// <exception cref="ArgumentNullException"/>
    public static void ValidateAndThrowSafe<T>(this IValidator<T> validator, [NotNull] T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        validator.ValidateAndThrow(instance);
    }
}
