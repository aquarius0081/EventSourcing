using EventSourcing.Shared.Events;

namespace Worker.Balance.Aggregates
{
    public class AccountAggregate
    {
        public int AggregateId { get; set; }

        public decimal Balance { get; set; }

        public int Version { get; set; }

        public void Apply(AccountCreatedEvent accountCreatedEvent)
        {
            AggregateId = accountCreatedEvent.AccountId;
            Balance = accountCreatedEvent.Balance;
            Version = accountCreatedEvent.Version;
        }

        public void Apply(AccountWithdrawnEvent accountWithdrawnEvent)
        {
            Balance -= accountWithdrawnEvent.Amount;
            Version = accountWithdrawnEvent.Version;
        }

        public void Apply(AccountDepositedEvent accountDepositedEvent)
        {
            Balance += accountDepositedEvent.Amount;
            Version = accountDepositedEvent.Version;
        }
    }
}
