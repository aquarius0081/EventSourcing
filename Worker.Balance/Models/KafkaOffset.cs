namespace Worker.Balance.Models
{
    public class KafkaOffset
    {
        public int Id { get; set; }

        public long LastProcessedMessageId { get; set; }
    }
}
