namespace EventSourcing.Shared.Settings
{
    public class KafkaProducerSettings
    {
        public string BootstrapServers { get; set; }

        public string EventTopic { get; set; }

        public string CommandTopic { get; set; }
    }
}
