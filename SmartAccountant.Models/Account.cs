namespace SmartAccountant.Models;

public abstract class Account
{
    public Guid Id { get; init; }

    public Bank Bank { get; init; }

    public string? FriendlyName { get; init; }

    public abstract BalanceType NormalBalance { get; }
}
public enum Bank
{
    Unknown = 0,
    GarantiBBVA = 1,
}

public enum BalanceType
{
    Debit = 0,
    Credit = 1,
}
