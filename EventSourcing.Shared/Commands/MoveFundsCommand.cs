namespace EventSourcing.Shared.Commands
{
    public class MoveFundsCommand : BaseCommand
    {
        public override string Type { get; set; } = nameof(MoveFundsCommand);

        public int SourceAccountId { get; set; }

        public int TargetAccountId { get; set; }

        public CurrencyTypes CurrencyType { get; set; }

        public decimal Amount { get; set; }
    }
}
