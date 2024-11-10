namespace SmartAccountant.Models
{
    public struct MonetaryValue
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }
    }

    public enum Currency
    {
        USD = 0,
        EUR = 1,
        TRY = 2,
    }
}
