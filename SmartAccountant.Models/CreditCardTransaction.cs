namespace SmartAccountant.Models;

public record class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}


public enum ProvisionState : byte
{
    Finalized = 0,
    Open = 1
}
