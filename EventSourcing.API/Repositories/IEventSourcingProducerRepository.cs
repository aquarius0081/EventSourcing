namespace EventSourcing.API.Repositories
{
    public interface IEventSourcingProducerRepository
    {
        public void AddCommand<T>(T command) where T : class;
    }
}
