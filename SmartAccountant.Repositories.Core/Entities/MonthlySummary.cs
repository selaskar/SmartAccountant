using Microsoft.EntityFrameworkCore;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Repositories.Core.Entities;

[Index(nameof(UserId), nameof(Month))]
internal class MonthlySummary
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateOnly Month { get; set; }

    public SummaryState State { get; set; }


    public virtual IList<CurrencySummary> CurrenciesS { get; set; } = [];
}
