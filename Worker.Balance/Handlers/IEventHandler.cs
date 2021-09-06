using EventSourcing.Shared.Events;

namespace Worker.Balance.Handlers
{
    public interface IEventHandler
    {
        void HandleAccountCreatedEvent(AccountCreatedEvent concreteEvent);
        void HandleAccountWithdrawnEvent(AccountWithdrawnEvent concreteEvent);
        void HandleAccountDepositedEvent(AccountDepositedEvent concreteEvent);
    }
}