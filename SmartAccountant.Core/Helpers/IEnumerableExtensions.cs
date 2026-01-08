using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Core.Helpers;

public static class IEnumerableExtensions
{
    //TODO: unit test
    /// <exception cref="OverflowException"/>
    /// <exception cref="ArgumentNullException"/>
    public static MonetaryValue Sum(this IEnumerable<MonetaryValue> values)
    {
        var list = values?.ToList();

        if (list == null || list.Count == 0)
            return default;

        return new MonetaryValue(list.Sum(v => v.Amount), list.First().Currency);
    }
}
