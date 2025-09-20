using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies;

internal static class SharedStringTableExtensions
{
    public static string GetNextReference(this SharedStringTable stringTable)
    {
        return stringTable.ChildElements.Count.ToString(CultureInfo.InvariantCulture);
    }
}
