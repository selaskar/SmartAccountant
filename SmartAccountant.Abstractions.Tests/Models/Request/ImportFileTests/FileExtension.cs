using SmartAccountant.Abstractions.Models.Request;

namespace SmartAccountant.Abstractions.Tests.Models.Request.ImportFileTests;

[TestClass]
public class FileExtension
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("  ")]
    [DataRow(".")]
    [DataRow("..")]
    [DataRow("...")]
    [DataRow(" .")]
    [DataRow(". ")]
    [DataRow(" . ")]
    [DataRow(" .  ")]
    [DataRow("a. ")]
    [DataRow("a.  ")]
    public void ReturnNullForEmptyExtension(string? fileName)
    {
        // Arrange
        ImportFile sut = new()
        {
            FileName = fileName!,
            ContentType = null!,
            OpenReadStream =null!
        };

        // Act
        string? extension = sut.FileExtension;

        // Assert
        Assert.IsNull(extension);
    }

    [TestMethod]
    [DataRow("a.a", ".a")]
    [DataRow("a. a", ".a")]
    [DataRow("a.a ", ".a")]
    [DataRow("a. a ", ".a")]
    [DataRow("a.aa", ".aa")]
    [DataRow("a.aa ", ".aa")]
    [DataRow("a.A ", ".A")]
    public void ReturnCorrectFileExtension(string fileName, string expected)
    {
        // Arrange
        ImportFile sut = new()
        {
            FileName = fileName,
            ContentType = null!,
            OpenReadStream = null!
        };

        // Act
        string? extension = sut.FileExtension;

        // Assert
        Assert.AreEqual(expected, extension);
    }
}
