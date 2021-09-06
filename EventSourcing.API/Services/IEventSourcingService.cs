using EventSourcing.Shared;

namespace EventSourcing.API.Services
{
    public interface IEventSourcingService
    {
        public void CreateAccount(AccountDto account);

        public void MoveFunds(MoveFundsDto moveFundsDto);

        public string GetAllEvents();
    }
}
