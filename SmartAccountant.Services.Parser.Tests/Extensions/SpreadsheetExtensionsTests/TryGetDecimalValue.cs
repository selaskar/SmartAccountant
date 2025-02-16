using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Services.Parser.Extensions;

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
}
