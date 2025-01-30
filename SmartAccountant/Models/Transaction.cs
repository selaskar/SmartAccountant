namespace SmartAccountant.Models;

public abstract class Transaction
{
    public Guid Id { get; set; }

    public required Account Account { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public MonetaryValue Amount { get; set; }
}

public class Deposit : Transaction
{

}

//TODO: category and sub-categories?
public enum TransactionType
{
    Deposit = 0,
    Withdrawal = 1,
    TransferIn = 2,
    TransferOut = 3,
    Expense = 4,
    Installment
}
