using System.Collections.Generic;
using Worker.Transaction.Models;

namespace Worker.Transaction.Services
{
    public interface IMoneyTransactionService
    {
        void AddTransaction(MoneyTransactionDto transactionDto);

        List<MoneyTransactionDto> GetAllTransactions();
    }
}
