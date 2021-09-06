using EventSourcing.Shared;
using EventSourcing.Shared.Events;
using Microsoft.Extensions.Logging;
using Worker.Balance.Services;

namespace Worker.Balance.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly ILogger<EventHandler> logger;
        private readonly IAccountBalanceService accountBalanceService;

        public EventHandler(ILogger<EventHandler> logger, IAccountBalanceService accountBalanceService)
        {
            this.logger = logger;
            this.accountBalanceService = accountBalanceService;
        }

        public void HandleAccountCreatedEvent(AccountCreatedEvent concreteEvent)
        {
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountCreatedEvent)} call: Start");
            var accountDto = new AccountDto
            {
                AccountId = concreteEvent.AccountId,
                AccountName = concreteEvent.AccountName,
                Balance = concreteEvent.Balance
            };
            accountBalanceService.CreateAccount(accountDto);
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountCreatedEvent)} call: End");
        }

        public void HandleAccountWithdrawnEvent(AccountWithdrawnEvent concreteEvent)
        {
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountWithdrawnEvent)} call: Start");
            accountBalanceService.UpdateAccountBalance(concreteEvent.AccountId, -concreteEvent.Amount);
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountWithdrawnEvent)} call: End");
        }

        public void HandleAccountDepositedEvent(AccountDepositedEvent concreteEvent)
        {
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountDepositedEvent)} call: Start");
            accountBalanceService.UpdateAccountBalance(concreteEvent.AccountId, concreteEvent.Amount);
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountDepositedEvent)} call: End");
        }
    }
}
