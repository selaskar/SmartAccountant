namespace SmartAccountant.Models;

public abstract class Transaction : BaseModel
{
    public required Guid StatementId { get; init; }

    public string? ReferenceNumber { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public MonetaryValue Amount { get; init; }

    public string? Note { get; init; }
}

//TODO: category and sub-categories?
public enum TransactionType
{
    Deposit = 0,
    Withdrawal = 1,
    TransferIn = 2,
    TransferOut = 3,
    Expense = 4,
    Installment = 5,
    DebtPayment = 6,
}
