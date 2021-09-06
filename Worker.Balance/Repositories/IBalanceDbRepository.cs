using System.Collections.Generic;
using Worker.Balance.Models;

namespace Worker.Balance.Repositories
{
    public interface IBalanceDbRepository
    {
        List<Account> GetAllBalances();

        void UpdateAccountBalance(int accountId, decimal amount);

        void CreateAccount(Account account);

        long GetLastProcessedMessageId();

        void UpdateLastProcessedMessageId(long newValue);
    }
}
