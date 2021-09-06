using System;

namespace EventSourcing.Shared.Events
{
    public class CreateAccountFailedEvent : BaseEvent
    {
        public override string Type { get; set; } = nameof(CreateAccountFailedEvent);

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public decimal Balance { get; set; }

        public Guid RelatedCommandId { get; set; }

        public string Reason { get; set; }
    }
}
