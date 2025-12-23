using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Tests.DebitImportServiceTests;

[TestClass]
public class DetectFinalized : Base
{
    [TestMethod]
    public void ReturnEmptyCollection()
    {
        // Act
        Transaction[] result = sut.DetectFinalized(null!, null!);

        // Assert
        Assert.HasCount(0, result);
    }
}
