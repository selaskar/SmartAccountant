namespace SmartAccountant.Models;

// From the perspective of domain, we don't need such a transaction type at the moment.
// However, this is useful when creating a statement based on transaction type in a factory class.
public record class CreditCardTransactionX : CreditCardTransaction
{

}
