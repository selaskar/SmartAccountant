﻿namespace SmartAccountant.Models;

public abstract record class Transaction : BaseModel
{
    public required Guid AccountId { get; init; }

    public string? ReferenceNumber { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public MonetaryValue Amount { get; init; }

    public string? Description { get; init; }

    public string? PersonalNote { get; init; }
}

//TODO: category and sub-categories? Flags?
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
