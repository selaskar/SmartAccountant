using SmartAccountant.Services.Parser.Helpers;

namespace SmartAccountant.Services.Parser.Tests.Helpers.RoundHelperTests;

[TestClass]
public class Round
{
    [TestMethod]
    [DataRow(1.00005, 1.0001)]
    [DataRow(-1.00005, -1.0001)]
    public void RoundAwayFromZero(double input, double expected)
    {
        // Act
        decimal result = RoundHelper.Round((decimal)input);

        // Assert
        Assert.AreEqual((decimal)expected, result);
    }

    [TestMethod]
    [DataRow(1.000006, 1.0000)]
    [DataRow(1.00006, 1.0001)]
    [DataRow(1.0006, 1.0006)]
    [DataRow(-1.000006, -1.0000)]
    [DataRow(-1.00006, -1.0001)]
    [DataRow(-1.0006, -1.0006)]
    public void RoundTo4DecimalPlaces(double input, double expected)
    {
        // Act
        decimal result = RoundHelper.Round((decimal)input);

        // Assert
        Assert.AreEqual((decimal)expected, result);
    }
}
