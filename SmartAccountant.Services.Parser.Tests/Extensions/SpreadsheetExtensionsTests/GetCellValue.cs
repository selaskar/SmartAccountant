using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Services.Parser.Extensions;

namespace SmartAccountant.Services.Parser.Tests.Extensions.SpreadsheetExtensionsTests;

[TestClass]
public class GetCellValue
{
    [TestMethod]
    public void ReturnEmptyStringForMissingCellValue()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = null
        };

        // Act
        string result = cell.GetCellValue(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ReturnEmptyStringForNullCellValue()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue(null!)
        };

        // Act
        string result = cell.GetCellValue(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ReturnCorrectValueForBasicString()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("Test Value"),
            DataType = CellValues.String,
        };

        // Act
        string result = cell.GetCellValue(null);

        // Assert
        Assert.AreEqual("Test Value", result);
    }

    [TestMethod]
    public void ThrowArgumentNullExceptionForMissingStringTable()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("0"),
            DataType = CellValues.SharedString,
        };

        // Act, Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => cell.GetCellValue(null));
    }

    [TestMethod]
    public void ReturnCorrectValueForSharedString()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("0"),
            DataType = CellValues.SharedString,
        };

        SharedStringTable stringTable = new();
        stringTable.AppendChild(new SharedStringItem(new Text("Shared String Value")));

        // Act
        string result = cell.GetCellValue(stringTable);

        // Assert
        Assert.AreEqual("Shared String Value", result);
    }
}
