using Moq;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Tests.ParseStrategies.AbstractGarantiStatementParseStrategyTests;

[TestClass]
public class Cast
{
    [TestMethod]
    public void ThrowInvalidCastExceptionForWrongStatementType()
    {
        // Arrange
        Mock<Statement<DebitTransaction>> mockStatement = new();

        // Act, Assert
        Assert.ThrowsExactly<InvalidCastException>(() => AbstractGarantiStatementParseStrategy.Cast<DebitStatement>(mockStatement.Object));
    }

    [TestMethod]
    public void ReturnStatementForCorrectType()
    {
        // Arrange
        Statement statement = new DebitStatement();

        // Act
        var result = AbstractGarantiStatementParseStrategy.Cast<DebitStatement>(statement);

        // Assert
        Assert.AreSame(statement, result);
    }
}
