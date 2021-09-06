using System.Collections.Generic;
using System.Linq;
using Worker.Balance.Data;
using Worker.Balance.Models;
using Microsoft.Extensions.Logging;

namespace Worker.Balance.Repositories
{
    public class BalanceDbRepository : IBalanceDbRepository
    {
        private readonly BalanceContext context;
        private readonly ILogger<BalanceDbRepository> logger;

        public BalanceDbRepository(BalanceContext context,
            ILogger<BalanceDbRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void CreateAccount(Account account)
        {
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(CreateAccount)} call: Start");
            context.Accounts.Add(account);
            context.SaveChanges();
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(CreateAccount)} call: End");
        }

        public List<Account> GetAllBalances()
        {
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(GetAllBalances)} called");
            return context.Accounts.ToList();
        }

        public void UpdateAccountBalance(int accountId, decimal amount)
        {
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(UpdateAccountBalance)} call: Start");
            var targetAccount = context.Accounts.FirstOrDefault(b => b.AccountId == accountId);
            if (targetAccount != null)
            {
                targetAccount.Balance += amount;
                context.SaveChanges();
            }
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(UpdateAccountBalance)} call: End");
        }

        public long GetLastProcessedMessageId()
        {
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(GetLastProcessedMessageId)} called");
            return context.KafkaOffsets.First().LastProcessedMessageId;
        }

        public void UpdateLastProcessedMessageId(long newValue)
        {
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(UpdateLastProcessedMessageId)} call: Start");
            context.KafkaOffsets.First().LastProcessedMessageId = newValue;
            context.SaveChanges();
            logger.LogInformation($"{nameof(BalanceDbRepository)}.{nameof(UpdateLastProcessedMessageId)} call: End");
        }
    }
}
