using EventSourcing.Shared;
using EventSourcing.Shared.Events;
using System.Collections.Generic;

namespace Worker.Balance.Services
{
    public interface IAccountBalanceService
    {
        List<AccountDto> GetBalances();

        void UpdateAccountBalance(int accountId, decimal amount);

        void CreateAccount(AccountDto account);

        void UpdateTransactions(AccountWithdrawnEvent accountWithdrawnEvent, AccountDepositedEvent accountDepositedEvent);

        void FailMoveFunds(MoveFundsFailedEvent moveFundsFailedEvent);

        long GetLastProcessedMessageId();

        void UpdateLastProcessedMessageId(long newValue);

        void AddAccountCreatedEvent(AccountCreatedEvent accountCreatedEvent);

        void FailCreateAccount(CreateAccountFailedEvent createAccountFailedEvent);
    }
}
