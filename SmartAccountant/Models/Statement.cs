namespace SmartAccountant.Models
{
    public class Statement
    {
        public Guid Id { get; set; }

        public Guid Account { get; set; }
        public DateTimeOffset PeriodStart { get; set; }

        public DateTimeOffset PeriodEnd { get; set; }

        public required IList<Transaction> Transactions { get; set; }
    }
}
