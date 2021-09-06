namespace EventSourcing.Shared.Settings
{
    public class KafkaConsumerSettings
    {
        public string EventGroupId { get; set; }

        public string BootstrapServers { get; set; }

        public string EventTopic { get; set; }

        public string CommandTopic { get; set; }

        public string CommandGroupId { get; set; }

        public int PartitionsCount { get; set; }
    }
}
