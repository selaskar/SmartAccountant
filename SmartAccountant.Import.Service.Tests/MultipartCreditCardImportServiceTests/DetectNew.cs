using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Import.Service.Tests.MultipartCreditCardImportServiceTests;

[TestClass]
public class DetectNew : Base
{
    [TestMethod]
    public void ThrowImportExceptionForWrongStatement()
    {
        // Arrange
        DebitStatement statement = new();

        // Act, Assert
        Assert.ThrowsExactly<ImportException>(() => sut.DetectNew(statement, null!));
    }

    [TestMethod]
    public void Succeed()
    {
        // Arrange
        var timestamp = new DateTimeOffset(new DateOnly(2025, 09, 19), TimeOnly.MinValue, TimeSpan.Zero);
        string description1 = "Description 1";
        MonetaryValue amount = new(10, Currency.USD);
        ProvisionState state = ProvisionState.Finalized;

        SharedStatement statement = new()
        {
            Transactions = [ new CreditCardTransaction()
            {
                AccountId = Guid.Empty,
                Timestamp = timestamp,
                Description = description1,
                Amount = amount,
                ProvisionState = state
            }],
            SecondaryTransactions = [ new CreditCardTransaction()
            {
                AccountId = Guid.Empty,
                Timestamp = timestamp,
                Description = "Description 2",
                Amount = amount,
                ProvisionState = state
            }]
        };

        CreditCardTransaction[] existingTransactions = [ new CreditCardTransaction()
        {
            AccountId = Guid.Empty,
            Timestamp = timestamp,
            Description = description1,
            Amount = amount,
            ProvisionState = state
        }];

        // Act
        Transaction[] result = sut.DetectNew(statement, existingTransactions);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual("Description 2", result[0].Description);
    }
}
