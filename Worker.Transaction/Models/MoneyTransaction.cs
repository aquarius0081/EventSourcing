using EventSourcing.Shared;
using System;

namespace Worker.Transaction.Models
{
    public class MoneyTransaction
    {
        public int Id { get; set; }

        public TransactionTypes TransactionType { get; set; }

        public int AccountId { get; set; }

        public CurrencyTypes CurrencyType { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
