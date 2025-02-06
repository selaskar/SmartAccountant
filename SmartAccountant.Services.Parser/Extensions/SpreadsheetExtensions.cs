using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SmartAccountant.Services.Parser.Extensions;

internal static class SpreadsheetExtensions
{
    internal static string GetCellValue(this Row row, int index, SharedStringTable stringTable)
    {
        return row.Descendants<Cell>().ElementAt(index).GetCellValue(stringTable);
    }

    internal static string GetCellValue(this Cell cell, SharedStringTable stringTable)
    {
        if (cell.CellValue?.Text is null)
            return string.Empty;

        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            var stringReference = int.Parse(cell.CellValue.Text, CultureInfo.InvariantCulture);
            return stringTable.ElementAt(stringReference).InnerText;
        }

        return cell.CellValue.Text;
    }
}
