namespace EventSourcing.Shared
{
    public class MoveFundsDto
    {
        public int SourceAccountId { get; set; }

        public int TargetAccountId { get; set; }

        public CurrencyTypes CurrencyType { get; set; }

        public decimal Amount { get; set; }
    }
}
