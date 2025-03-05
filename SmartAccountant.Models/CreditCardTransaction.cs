using System.Diagnostics.CodeAnalysis;

namespace SmartAccountant.Models;

public record class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}


[SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "We map this enum to a database column")]
public enum ProvisionState : byte
{
    Finalized = 0,
    Open = 1
}
