using System.Globalization;
using System.Text;

namespace SmartAccountant.Core.Helpers;

public static class CompositeFormatExtensions
{
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentNullException"/>
    public static string FormatMessage(this CompositeFormat format, params object?[] parameters) =>
        string.Format(CultureInfo.InvariantCulture, format, parameters);
}
