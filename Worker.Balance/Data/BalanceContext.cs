using Microsoft.EntityFrameworkCore;
using Worker.Balance.Models;

namespace Worker.Balance.Data
{
    public class BalanceContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<KafkaOffset> KafkaOffsets { get; set; }

        public BalanceContext(DbContextOptions<BalanceContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KafkaOffset>().HasData(
                new KafkaOffset { Id = 1, LastProcessedMessageId = -1 }
            );
        }
    }
}
