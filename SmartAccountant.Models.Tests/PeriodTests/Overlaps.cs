namespace SmartAccountant.Models.Tests.PeriodTests;

[TestClass]
public class Overlaps
{
    [TestMethod]
    [DataRow("2025-02-28T00:00:00.0000000+00:00", true)]
    [DataRow("2025-02-28T00:00:00.0000001+00:00", false)]
    [DataRow("2025-02-28T00:00:00.0000000-00:01", false)]
    [DataRow("2025-02-28T00:00:00.0000000+00:01", true)]
    [DataRow("2025-02-27T23:00:00.0000000-01:00", true)]
    [DataRow("2025-02-28T01:00:00.0000000+01:00", true)]
    [DataRow("2025-02-28T01:00:01.0000000+01:00", false)]
    [DataRow("2025-02-01T00:00:00.0000000+00:00", true)]
    [DataRow("2025-02-01T00:00:00.0000001+00:00", true)]
    [DataRow("2025-02-01T00:00:00.0000000+00:01", false)]
    [DataRow("2025-02-01T00:00:00.0000000-00:01", true)]
    public void ReturnCorrectResultForPeriodWithEndDate(string dateString, bool expected)
    {
        // Arrange
        var period = new Period
        {
            ValidFrom = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            ValidTo = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero)
        };
        var date = DateTimeOffset.ParseExact(dateString, "O", null);

        // Act
        var result = period.Overlaps(date);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ReturnFalseForInvalidPeriod()
    {
        // Arrange
        var period = new Period
        {
            ValidFrom = new DateTimeOffset(2025, 02, 03, 0, 0, 0, TimeSpan.Zero),
            ValidTo = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero)
        };
        var date = new DateTimeOffset(2025, 02, 02, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = period.Overlaps(date);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("2025-02-01T00:00:00.0000000+00:00", true)]
    [DataRow("2025-02-01T00:00:00.0000000-00:01", true)]
    [DataRow("2025-02-01T00:00:00.0000000+00:01", false)]
    [DataRow("2025-02-01T00:01:00.0000000+00:00", true)]
    [DataRow("2025-02-01T00:01:00.0000000-00:01", true)]
    [DataRow("2025-02-01T00:01:00.0000000+00:01", true)]
    [DataRow("2025-02-01T00:01:00.0000000+00:02", false)]
    public void ReturnCorrectResultForPeriodWithNoEndDate(string dateString, bool expected)
    {
        // Arrange
        var period = new Period
        {
            ValidFrom = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            ValidTo = null
        };
        var date = DateTimeOffset.ParseExact(dateString, "O", null);

        // Act
        var result = period.Overlaps(date);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("2025-02-01T00:00:00.0000000+00:00", true)]
    [DataRow("2025-02-01T00:00:01.0000000+00:00", true)]
    [DataRow("2025-01-31T00:00:00.0000000+00:00", true)]
    public void ReturnCorrectResultForPeriodsWithoutEndDate(string dateString, bool expected)
    {
        // Arrange
        var period = new Period
        {
            ValidFrom = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            ValidTo = null
        };

        var period2 = new Period
        {
            ValidFrom = DateTimeOffset.ParseExact(dateString, "O", null),
            ValidTo = null
        };

        // Act
        var result = period.Overlaps(period2);

        // Assert
        Assert.AreEqual(expected, result);
    }


    [TestMethod]
    [DataRow("2025-02-01T00:00:00.0000000+00:00", "2025-02-01T00:00:00.0000000+00:00", false)]
    [DataRow("2025-02-01T00:00:00.0000000+00:00", "2025-02-01T00:00:00.0000001+00:00", true)]
    [DataRow("2025-02-28T00:00:00.0000000+00:00", "2025-02-28T00:00:00.0000001+00:00", false)]
    [DataRow("2025-02-27T00:00:00.0000000+00:00", "2025-02-28T00:00:00.0000000+00:00", true)]
    public void ReturnCorrectResultForPeriodsWithEndDates(string dateFrom, string dateTo, bool expected)
    {
        // Arrange
        var period = new Period
        {
            ValidFrom = new DateTimeOffset(2025, 02, 01, 0, 0, 0, TimeSpan.Zero),
            ValidTo = new DateTimeOffset(2025, 02, 28, 0, 0, 0, TimeSpan.Zero),
        };

        var period2 = new Period
        {
            ValidFrom = DateTimeOffset.ParseExact(dateFrom, "O", null),
            ValidTo = DateTimeOffset.ParseExact(dateTo, "O", null),
        };

        // Act
        var result = period.Overlaps(period2);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
