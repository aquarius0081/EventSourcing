using System;

namespace EventSourcing.Shared.Events
{
    public class BaseEvent
    {
        public Guid Id { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public virtual string Type { get; set; }

        public int Version { get; set; }
    }
}
