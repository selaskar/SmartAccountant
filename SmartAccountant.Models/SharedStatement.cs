namespace SmartAccountant.Models;

public record class SharedStatement : CreditCardStatement
{
    public string? CardNumber1 { get; set; }

    public string? CardNumber2 { get; set; }

    public Guid? DependentAccountId { get; set; }

    public IList<CreditCardTransaction> SecondaryTransactions { get; init; } = [];
}
