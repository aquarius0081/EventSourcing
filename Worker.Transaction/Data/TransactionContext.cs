using Microsoft.EntityFrameworkCore;
using Worker.Transaction.Models;

namespace Worker.Transaction.Data
{
    public class TransactionContext : DbContext
    {
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; }

        public TransactionContext(DbContextOptions<TransactionContext> options)
            : base(options) { }
    }
}
