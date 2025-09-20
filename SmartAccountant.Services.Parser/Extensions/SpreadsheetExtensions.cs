using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SmartAccountant.Services.Parser.Extensions;

internal static class SpreadsheetExtensions
{
    /// <exception cref="ArgumentOutOfRangeException"/>
    internal static Cell GetCell(this Row row, int index)
    {
        return row.Descendants<Cell>().ElementAt(index);
    }

    /// <summary>When <see cref="CellType.DataType"/> is <see cref="CellValues.SharedString"/>, 
    /// a valid <paramref name="stringTable"/> must be supplied.</summary>
    /// <exception cref="ArgumentNullException" />
    internal static string GetCellValue(this Cell cell, SharedStringTable? stringTable)
    {
        if (cell.CellValue?.Text is null)
            return string.Empty;

        if (cell.DataType?.Value != CellValues.SharedString)
            return cell.CellValue.Text;

        ArgumentNullException.ThrowIfNull(stringTable);

        var stringReference = int.Parse(cell.CellValue.Text, CultureInfo.InvariantCulture);
        return stringTable.ElementAt(stringReference).InnerText;
    }

    internal static bool TryGetDecimalValue(this Cell cell, [NotNullWhen(true)] out decimal? value)
    {
        if (cell.DataType?.Value == CellValues.SharedString)
        {
            //This type of cells include integer values as pointers to shared strings.
            //We should not interpret them as numeric values.
            //Consumer should use the other overload.
            value = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(cell.CellValue?.Text))
        {
            value = null;
            return false;
        }

        if (cell.CellValue.TryGetDecimal(out decimal value2))
        {
            value = value2;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    /// <exception cref="ArgumentNullException" />
    internal static bool TryGetDecimalValue(this Cell cell, SharedStringTable stringTable, IFormatProvider formatProvider, [NotNullWhen(true)] out decimal? value)
    {
        if (cell.DataType?.Value != CellValues.SharedString)
        {
            value = null;
            return false;
        }

        string amountString = cell.GetCellValue(stringTable);

        if (string.IsNullOrWhiteSpace(amountString))
        {
            value = null;
            return false;
        }

        if (decimal.TryParse(amountString, formatProvider, out decimal value2))
        {
            value = value2;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }
}
