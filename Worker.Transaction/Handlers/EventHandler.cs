using EventSourcing.Shared.Events;
using Microsoft.Extensions.Logging;
using Worker.Transaction.Models;
using Worker.Transaction.Services;

namespace Worker.Transaction.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly ILogger<EventHandler> logger;
        private readonly IMoneyTransactionService moneyTransactionService;

        public EventHandler(ILogger<EventHandler> logger, IMoneyTransactionService moneyTransactionService)
        {
            this.logger = logger;
            this.moneyTransactionService = moneyTransactionService;
        }

        public void HandleAccountWithdrawnEvent(AccountWithdrawnEvent concreteEvent)
        {
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountWithdrawnEvent)} call: Start");
            var transactionToWithdrawFromSourceAccount = new MoneyTransactionDto
            {
                AccountId = concreteEvent.AccountId,
                Amount = concreteEvent.Amount,
                TransactionType = TransactionTypes.Withdraw,
                CurrencyType = concreteEvent.CurrencyType,
                TransactionDate = concreteEvent.CreatedDateTimeUtc
            };

            moneyTransactionService.AddTransaction(transactionToWithdrawFromSourceAccount);
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountWithdrawnEvent)} call: End");
        }

        public void HandleAccountDepositedEvent(AccountDepositedEvent concreteEvent)
        {
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountDepositedEvent)} call: Start");
            var transactionToDepositToTargetAccount = new MoneyTransactionDto
            {
                AccountId = concreteEvent.AccountId,
                Amount = concreteEvent.Amount,
                TransactionType = TransactionTypes.Deposit,
                CurrencyType = concreteEvent.CurrencyType,
                TransactionDate = concreteEvent.CreatedDateTimeUtc
            };

            moneyTransactionService.AddTransaction(transactionToDepositToTargetAccount);
            logger.LogInformation($"{nameof(EventHandler)}.{nameof(HandleAccountDepositedEvent)} call: End");
        }
    }
}
