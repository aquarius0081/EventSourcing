using System;

namespace EventSourcing.Shared.Events
{
    public class AccountCreatedEvent : BaseEvent
    {
        public override string Type { get; set; } = nameof(AccountCreatedEvent);

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public decimal Balance { get; set; }

        public Guid RelatedCommandId { get; set; }
    }
}
