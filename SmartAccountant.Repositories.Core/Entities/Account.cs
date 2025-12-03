using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Repositories.Core.Entities;

[Index(nameof(HolderId))]
internal abstract class Account
{
    public Guid Id { get; set; }

    public Guid HolderId { get; set; }

    public Bank Bank { get; set; }

    [StringLength(50)]
    public string? FriendlyName { get; set; }

    [NotMapped]
    public abstract BalanceType NormalBalance { get; }


    public virtual IList<Transaction> Transactions { get; set; } = [];

    public virtual IList<Statement> Statements { get; set; } = [];
}
