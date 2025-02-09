using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Services.Parser.Extensions;

namespace SmartAccountant.Services.Parser.Tests.Extensions.SpreadsheetExtensionsTests;

[TestClass]
public class GetCell
{
    [TestMethod]
    public void ThrowArgumentOutOfRangeExceptionForInvalidIndex()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("Test Value"),
            DataType = CellValues.String,
        };

        Row row = new();
        row.Append(cell);

        // Act, Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => row.GetCell(1));
    }

    [TestMethod]
    public void ReturnCorrectCellForValidIndex()
    {
        // Arrange
        Cell cell = new()
        {
            CellValue = new CellValue("Test Value"),
            DataType = CellValues.String,
        };

        Row row = new();
        row.Append(cell);

        // Act
        Cell result = row.GetCell(0);

        // Assert
        Assert.AreEqual(cell, result);
    }
}
