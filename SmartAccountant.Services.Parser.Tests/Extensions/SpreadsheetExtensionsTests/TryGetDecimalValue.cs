using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Tests.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.Extensions.SpreadsheetExtensionsTests;

[TestClass]
public class TryGetDecimalValue
{
    [TestMethod]
    public void ReturnFalseForSharedStringCells()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("38"),
            DataType = CellValues.SharedString
        };

        // Act
        bool result = cell.TryGetDecimalValue(out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void ReturnFalseForMissingCellValue()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = null
        };

        // Act
        bool result = cell.TryGetDecimalValue(out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void ReturnFalseForNullOrEmptyCellValue(string input)
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue(input)
        };

        // Act
        bool result = cell.TryGetDecimalValue(out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("1.a")]
    [DataRow("a.1")]
    [DataRow(".")]
    public void ReturnFalseForNonNumericCellValue(string input)
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue(input)
        };

        // Act
        bool result = cell.TryGetDecimalValue(out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow("0", 0)]
    [DataRow("1", 1)]
    [DataRow("1.", 1)]
    [DataRow(".1", 0.1)]
    [DataRow("1.1", 1.1)]
    [DataRow("-1.1", -1.1)]
    [DataRow("+1.1", 1.1)]
    public void ReturnTrueForNumericCellValue(string input, double expected)
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue(input)
        };

        // Act
        bool result = cell.TryGetDecimalValue(out decimal? value);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((decimal)expected, value);
    }

    /*
     * Overload(this Cell, SharedStringTable, IFormatProvider, out decimal?)
     */
    [TestMethod]
    public void ReturnFalseForMissingCellValue2()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = null
        };

        // Act
        bool result = cell.TryGetDecimalValue(stringTable: null!, formatProvider: null!, out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void ReturnFalseForNonSharedStringCells()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("38"),
            DataType = CellValues.InlineString
        };

        // Act
        bool result = cell.TryGetDecimalValue(stringTable: null!, formatProvider: null!, out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void ReturnFalseForNullOrEmptyCellValue2(string? text)
    {
        // Arrange
        SharedStringTable sharedStringTable = new();

        Cell cell = new()
        {
            CellValue = new CellValue(sharedStringTable.GetNextReference()),
            DataType = CellValues.SharedString
        };

        sharedStringTable.AppendChild(new SharedStringItem(new Text(text!)));

        // Act
        bool result = cell.TryGetDecimalValue(sharedStringTable, formatProvider: null!, out decimal? value);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    [DataRow("1,1", "tr-TR", 1.1)]
    [DataRow("1.000,1", "tr-TR", 1000.1)]
    [DataRow("1.1", "en-US", 1.1)]
    [DataRow("1,000.1", "en-US", 1000.1)]
    public void ReturnTrueForMatchedCulture(string text, string culture, double expected)
    {
        // Arrange
        SharedStringTable sharedStringTable = new();

        Cell cell = new()
        {
            CellValue = new CellValue(sharedStringTable.GetNextReference()),
            DataType = CellValues.SharedString
        };

        sharedStringTable.AppendChild(new SharedStringItem(new Text(text)));

        // Act
        bool result = cell.TryGetDecimalValue(sharedStringTable, CultureInfo.GetCultureInfo(culture), out decimal? value);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((decimal)expected, value);
    }
}
