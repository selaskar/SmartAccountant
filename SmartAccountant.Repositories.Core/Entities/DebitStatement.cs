using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class DebitStatement : Statement
{
    public Currency Currency { get; set; }
}
