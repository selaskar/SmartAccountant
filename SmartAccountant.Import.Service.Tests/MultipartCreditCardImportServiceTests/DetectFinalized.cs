using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.MultipartCreditCardImportServiceTests;

[TestClass]
public class DetectFinalized : Base
{
    [TestMethod]
    public void ReturnEmptyCollection()
    {
        // Act
        Transaction[] result = sut.DetectFinalized(null!, null!);

        // Assert
        Assert.AreEqual(0, result.Length);
    }
}
