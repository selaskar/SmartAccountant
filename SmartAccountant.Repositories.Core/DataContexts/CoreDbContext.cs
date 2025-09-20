using Microsoft.EntityFrameworkCore;
using SmartAccountant.Repositories.Core.Entities;

namespace SmartAccountant.Repositories.Core.DataContexts;

internal sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }

    public DbSet<Balance> Balances { get; set; }

    public DbSet<CreditCardLimit> CreditCardLimits { get; set; }

    public DbSet<CreditCard> CreditCards { get; set; }

    public DbSet<CreditCardStatement> CreditCardStatements { get; set; }

    public DbSet<CreditCardTransaction> CreditCardTransactions { get; set; }

    public DbSet<DebitStatement> DebitStatements { get; set; }

    public DbSet<DebitTransaction> DebitTransactions { get; set; }

    public DbSet<SavingAccount> SavingAccounts { get; set; }

    public DbSet<SharedStatement> SharedStatements { get; set; }

    public DbSet<StatementDocument> StatementDocuments { get; set; }

    public DbSet<Statement> Statements { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<VirtualCard> VirtualCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);

        modelBuilder.Entity<Account>().UseTptMappingStrategy();

        modelBuilder.Entity<Statement>().UseTptMappingStrategy();

        modelBuilder.Entity<Transaction>().UseTptMappingStrategy();
    }
}
