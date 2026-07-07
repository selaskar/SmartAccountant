using System.ComponentModel.DataAnnotations.Schema;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Repositories.Core.Entities;

internal class CurrencySummary
{
    public Guid Id { get; set; }
    
    public Guid MonthlySummaryId { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal RemainingBalancesTotal { get; set; }

    public Currency RemainingBalancesTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal OriginalLimitsTotal { get; set; }

    public Currency OriginalLimitsTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal IncomeTotal { get; set; }

    public Currency IncomeTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal ExpensesTotal { get; set; }

    public Currency ExpensesTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal InterestAndFeesTotal { get; set; }

    public Currency InterestAndFeesTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal PlannedExpensesTotal { get; set; }

    public Currency PlannedExpensesTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal LoansTotal { get; set; }

    public Currency LoansTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal SavingsTotal { get; set; }

    public Currency SavingsTotalCurrency { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal Net { get; set; }

    public Currency NetCurrency { get; set; }
}
