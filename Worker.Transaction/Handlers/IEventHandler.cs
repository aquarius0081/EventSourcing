using EventSourcing.Shared.Events;

namespace Worker.Transaction.Handlers
{
    public interface IEventHandler
    {
        void HandleAccountWithdrawnEvent(AccountWithdrawnEvent concreteEvent);
        void HandleAccountDepositedEvent(AccountDepositedEvent concreteEvent);
    }
}