using System.Text;
using SmartAccountant.Core.Helpers;

namespace SmartAccountant.Core.Tests.Helpers.CompositeFormatExtensionsTests;

[TestClass]
public class FormatMessage
{
    [TestMethod]
    public void CorrectlyFormatMessage()
    {
        // Arrange
        CompositeFormat sut = CompositeFormat.Parse("Test {0}:{1}");

        // Act
        string result = sut.FormatMessage("x", "y");

        // Assert
        Assert.AreEqual("Test x:y", result);
    }
}
