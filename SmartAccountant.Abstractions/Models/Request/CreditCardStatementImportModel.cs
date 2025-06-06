﻿namespace SmartAccountant.Abstractions.Models.Request;

public class CreditCardStatementImportModel : AbstractStatementImportModel
{
    /// <summary>
    /// Outstanding debt from previous periods.
    /// </summary>
    public decimal? RolloverAmount { get; init; }

    public decimal TotalDueAmount { get; init; }

    public decimal? MinimumDueAmount { get; init; }

    public decimal? TotalFees { get; init; }

    public DateTimeOffset DueDate { get; set; }
}
