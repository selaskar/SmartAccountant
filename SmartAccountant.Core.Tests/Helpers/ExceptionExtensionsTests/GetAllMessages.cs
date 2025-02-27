using Moq;
using SmartAccountant.Core.Helpers;

namespace SmartAccountant.Core.Tests.Helpers.ExceptionExtensionsTests;

[TestClass]
public class GetAllMessages
{
    [TestMethod]
    public void ThrowArgumentNullExceptionForNullException()
    {
        // Act, Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => ExceptionExtensions.GetAllMessages(null!));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void EmptyStringForMissingExceptionMessage(string? message)
    {
        // Arrange
        Mock<Exception> mock = new();

        mock.Setup(x => x.Message).Returns(message!);

        // Act
        string result = mock.Object.GetAllMessages();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void OriginalMessageForSingleException()
    {
        // Arrange
        var exception = new ArgumentException("test");

        // Act
        string result = exception.GetAllMessages();

        // Assert
        Assert.AreEqual("test", result);
    }

    [TestMethod]
    public void CombineMessagesFromInnerExceptions()
    {
        // Arrange
        var inner = new ArgumentException("test inner");
        var outer = new InvalidOperationException("test outer", inner);

        // Act
        string result = outer.GetAllMessages();

        // Assert
        Assert.AreEqual("test outer" + Environment.NewLine + "test inner", result);
    }

    [TestMethod]
    public void CombineMessagesFromAggregateException()
    {
        // Arrange
        var inner1 = new ArgumentException("test inner 1");
        var inner2 = new InvalidOperationException("test inner 2", inner1);
        var inner3 = new ArgumentException("test inner 3");
        var outer = new AggregateException("aggregate outer", inner2, inner3);

        // Act
        string result = outer.GetAllMessages();

        // Assert
        Assert.AreEqual("aggregate outer (test inner 2) (test inner 3)" + Environment.NewLine + "test inner 1", result);
    }
}
