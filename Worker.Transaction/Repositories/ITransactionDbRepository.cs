using System.Collections.Generic;
using Worker.Transaction.Models;

namespace Worker.Transaction.Repositories
{
    public interface ITransactionDbRepository
    {
        void SaveTransaction(MoneyTransaction transaction);

        List<MoneyTransaction> GetAllTransactions();
    }
}
