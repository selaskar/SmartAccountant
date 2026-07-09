namespace SmartAccountant.Models;

public record class SharedStatement : AbstractCreditCardStatement<CreditCardTransactionX>
{
    public string? CardNumber1 { get; set; }

    public string? CardNumber2 { get; set; }

    public Guid? DependentAccountId { get; set; }

    public IList<CreditCardTransactionX> SecondaryTransactions { get; init; } = [];
}
