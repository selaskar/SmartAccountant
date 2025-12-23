using System.Globalization;
using FluentValidation;
using SmartAccountant.Import.Service.Extensions;

namespace SmartAccountant.Import.Service.Extensions;

internal static class IRuleBuilderExtensions
{
    extension<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
    {
        public IRuleBuilderOptions<T, TProperty> WithErrorCode(Enum errorCode)
        {
            return rule.WithErrorCode(errorCode.GetHashCode().ToString(CultureInfo.InvariantCulture));
        }
    }
}
