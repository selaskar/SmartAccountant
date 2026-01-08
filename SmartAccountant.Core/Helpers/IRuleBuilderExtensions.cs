using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentValidation;

namespace SmartAccountant.Core.Helpers;

[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Analyzer does not support new extension structure.")]
public static class IRuleBuilderExtensions
{
    extension<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
    {
        public IRuleBuilderOptions<T, TProperty> WithErrorCode(Enum errorCode)
        {
            ArgumentNullException.ThrowIfNull(errorCode);

            return rule.WithErrorCode(errorCode.GetHashCode().ToString(CultureInfo.InvariantCulture));
        }
    }
}
