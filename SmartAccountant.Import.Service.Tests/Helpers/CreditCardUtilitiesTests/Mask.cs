using SmartAccountant.Import.Service.Helpers;

namespace SmartAccountant.Import.Service.Tests.Helpers.CreditCardUtilitiesTests;

[TestClass]
public class Mask
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("1")]
    [DataRow("12345678")] //should not contain whitespaces.
    public void ReturnOriginalValueForShortNumbers(string? number)
    {
        // Act
        string result = CreditCardUtilities.Mask(number!);

        // Assert
        Assert.AreEqual(number, result);
    }

    [TestMethod]
    [DataRow(" 1234 5678 ", "12345678")]
    [DataRow("1234 * 5678", "1234*5678")]
    [DataRow("1234 **** **** 5678", "1234********5678")]
    public void ReturnCorrectMaskedValue(string number, string expected)
    {
        // Act
        string result = CreditCardUtilities.Mask(number!);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
