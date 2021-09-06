using System;

namespace EventSourcing.Shared.Events
{
    public class MoveFundsFailedEvent : BaseEvent
    {
        public override string Type { get; set; } = nameof(MoveFundsFailedEvent);

        public int SourceAccountId { get; set; }

        public int TargetAccountId { get; set; }

        public CurrencyTypes CurrencyType { get; set; }

        public decimal Amount { get; set; }

        public Guid RelatedCommandId { get; set; }

        public string Reason { get; set; }
    }
}
