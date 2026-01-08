using SmartAccountant.Models.Request;

namespace SmartAccountant.Abstractions.Tests.Models.Request.ImportFileTests;

[TestClass]
public sealed class Length
{
    [TestMethod]
    public void ReturnCorrectLengthForEmptyStream()
    {
        // Arrange
        ImportFile sut = new()
        {
            FileName = null!,
            ContentType = null!,
            OpenReadStream = () => new MemoryStream([])
        };

        // Act
        long length = sut.Length;

        // Assert
        Assert.AreEqual(0, length);
    }

    [TestMethod]
    public void ReturnCorrectLengthForStreamWithData()
    {
        // Arrange
        ImportFile sut = new()
        {
            FileName = null!,
            ContentType = null!,
            OpenReadStream = () => new MemoryStream([0, 1, 2])
        };

        // Act
        long length = sut.Length;

        // Assert
        Assert.AreEqual(3, length);
    }
}
