namespace SmartAccountant.Models;

public record class SharedStatement : CreditCardStatement, IStatement<XCreditCardTransaction> /* Call me the trickster. */
{
    public string? CardNumber1 { get; set; }

    public string? CardNumber2 { get; set; }

    public Guid? DependentAccountId { get; set; }

    public IList<CreditCardTransaction> SecondaryTransactions { get; init; } = [];

    IEnumerable<XCreditCardTransaction> IStatement<XCreditCardTransaction>.Transactions => Transactions.Cast<XCreditCardTransaction>();
}
