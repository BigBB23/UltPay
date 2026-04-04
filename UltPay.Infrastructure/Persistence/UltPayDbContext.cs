
using Microsoft.EntityFrameworkCore;
using UltPay.Domain.Entities;

namespace UltPay.Infrastructure.Persistence;

public class UltPayDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.Property(x => x.AvailableBalance).HasPrecision(18, 2);
            entity.Property(x => x.ReservedBalance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<LedgerEntry>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.BalanceBefore).HasPrecision(18, 2);
            entity.Property(x => x.BalanceAfter).HasPrecision(18, 2);
        });
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.Property(x => x.SourceAmount).HasPrecision(18, 2);
            entity.Property(x => x.DestinationAmount).HasPrecision(18, 2);
            entity.Property(x => x.FeeAmount).HasPrecision(18, 2);
            entity.Property(x => x.FxRate).HasPrecision(18, 6);
        });

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.Property(x => x.SourceAmount).HasPrecision(18, 2);
            entity.Property(x => x.DestinationAmount).HasPrecision(18, 2);
            entity.Property(x => x.FeeAmount).HasPrecision(18, 2);
            entity.Property(x => x.FxRate).HasPrecision(18, 6);
        });
    }
    public UltPayDbContext(DbContextOptions<UltPayDbContext> options) : base(options)
    {
    }
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<User> Users => Set<User>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferEvent> TransferEvents => Set<TransferEvent>();
}
