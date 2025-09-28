using SmartAccountant.Models;

namespace SmartAccountant.Core.Helpers;

public static class IEnumerableExtensions
{
    //TODO: unit test
    public static MonetaryValue Sum(this IEnumerable<MonetaryValue> values)
    {
        if (values ==null || !values.Any())
            return default;

        return new MonetaryValue(values.Sum(v => v.Amount), values.First().Currency);
    }
}
