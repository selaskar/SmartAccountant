using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}
