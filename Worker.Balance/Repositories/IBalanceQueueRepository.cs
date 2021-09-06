namespace Worker.Balance.Repositories
{
    public interface IBalanceQueueRepository
    {
        void AddEvent<T>(T @event) where T : class;
    }
}
