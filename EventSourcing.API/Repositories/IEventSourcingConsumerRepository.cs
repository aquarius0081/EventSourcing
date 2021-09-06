namespace EventSourcing.API.Repositories
{
    public interface IEventSourcingConsumerRepository
    {
        public string ConsumeAllEvents();
    }
}
