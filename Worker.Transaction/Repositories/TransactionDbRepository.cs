using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Worker.Transaction.Data;
using Worker.Transaction.Models;

namespace Worker.Transaction.Repositories
{
    public class TransactionDbRepository : ITransactionDbRepository
    {
        private readonly TransactionContext context;
        private readonly ILogger<TransactionDbRepository> logger;

        public TransactionDbRepository(TransactionContext context,
            ILogger<TransactionDbRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public List<MoneyTransaction> GetAllTransactions()
        {
            logger.LogInformation($"{nameof(TransactionDbRepository)}.{nameof(GetAllTransactions)} called");
            return context.MoneyTransactions.ToList();
        }

        public void SaveTransaction(MoneyTransaction transaction)
        {
            logger.LogInformation($"{nameof(TransactionDbRepository)}.{nameof(SaveTransaction)} call: Start");
            context.MoneyTransactions.Add(transaction);
            context.SaveChanges();
            logger.LogInformation($"{nameof(TransactionDbRepository)}.{nameof(SaveTransaction)} call: End");
        }
    }
}
