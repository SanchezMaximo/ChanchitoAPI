using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Administrator> Administrators => Set<Administrator>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().UseTpcMappingStrategy();

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(a => a.Balance).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(Category.MaxNameLength);

                // CustomerId is null for system categories, so this also keeps system names unique.
                // HasFilter(null) removes EF's default "IS NOT NULL" filter, which would skip the system rows.
                entity.HasIndex(c => new { c.CustomerId, c.Name })
                    .IsUnique()
                    .HasFilter(null);

                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(c => c.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.Property(e => e.Buy).HasPrecision(18, 4);
                entity.Property(e => e.Sell).HasPrecision(18, 4);

                // The same rate (source + provider timestamp) is never stored twice.
                entity.HasIndex(e => new { e.Source, e.UpdatedAt }).IsUnique();
            });
        }
    }
}