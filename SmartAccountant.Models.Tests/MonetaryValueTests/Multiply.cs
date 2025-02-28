namespace SmartAccountant.Models.Tests.MonetaryValueTests;

[TestClass]
public class Multiply
{
    [TestMethod]
    public void ReturnCorrectMonetaryValue()
    {
        // Arrange
        var monetaryValue = new MonetaryValue(10, Currency.USD);
        var multiplier = 2;

        // Act
        var result = MonetaryValue.Multiply(monetaryValue, multiplier);

        // Assert
        Assert.AreEqual(20, result.Amount);
        Assert.AreEqual(Currency.USD, result.Currency);
    }
}
