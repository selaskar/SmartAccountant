using SmartAccountant.Import.Service.Helpers;

namespace SmartAccountant.Import.Service.Tests.Helpers.CreditCardUtilitiesTests;

[TestClass]
public class CompareNumbersWithMasking
{
    [TestMethod]
    [DataRow(null, "1")]
    [DataRow("", null)]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow("", " ")]
    [DataRow("    ", "    ")]
    public void ReturnFalseWhenAnyOfNumbersEmpty(string? numberA, string? numberB)
    {
        // Act
        bool result = CreditCardUtilities.CompareNumbersWithMasking(numberA, numberB);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("1", "1", true)]
    [DataRow("1", "2", false)]
    [DataRow("1234", "1234", true)]
    [DataRow("1234", "1235", false)]
    [DataRow("12345678", "12345678", true)]
    [DataRow("1234 5678", "1234  5678", true)]
    [DataRow("1234*5678", "1234*5678", true)]
    [DataRow("123405678", "123495678", true)]
    [DataRow("1234*5678", "1234*5679", false)]
    public void ReturnCorrectResult(string numberA, string numberB, bool expected)
    {
        // Act
        bool result = CreditCardUtilities.CompareNumbersWithMasking(numberA, numberB);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
