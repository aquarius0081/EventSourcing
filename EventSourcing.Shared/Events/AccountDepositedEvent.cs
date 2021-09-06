using System;

namespace EventSourcing.Shared.Events
{
    public class AccountDepositedEvent : BaseEvent
    {
        public override string Type { get; set; } = nameof(AccountDepositedEvent);

        public int AccountId { get; set; }

        public CurrencyTypes CurrencyType { get; set; }

        public decimal Amount { get; set; }

        public Guid RelatedCommandId { get; set; }
    }
}
