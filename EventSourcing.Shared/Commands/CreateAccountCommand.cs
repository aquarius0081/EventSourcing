namespace EventSourcing.Shared.Commands
{
    public class CreateAccountCommand : BaseCommand
    {
        public override string Type { get; set; } = nameof(CreateAccountCommand);

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public decimal Balance { get; set; }
    }
}
