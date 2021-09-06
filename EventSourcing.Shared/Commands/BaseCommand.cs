using System;

namespace EventSourcing.Shared.Commands
{
    public class BaseCommand
    {
        public Guid Id { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public virtual string Type { get; set; }
    }
}
