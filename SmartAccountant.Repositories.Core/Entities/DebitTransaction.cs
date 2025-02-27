using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Entities;

internal sealed class DebitTransaction : Transaction
{
    [Column(TypeName = "decimal(19, 4)")]
    public decimal RemainingAmount { get; set; }

    public Currency RemainingAmountCurrency { get; set; }

    //TODO: probably unnecessary, as it is meaningless outside of statement file.
    public short Order { get; set; }
}
