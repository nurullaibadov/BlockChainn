using Blockchain.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Block> Blocks => Set<Block>();
        public DbSet<SmartContract> SmartContracts => Set<SmartContract>();
        public DbSet<ContractInteraction> ContractInteractions => Set<ContractInteraction>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Soft delete global filter
            builder.Entity<Wallet>().HasQueryFilter(w => !w.IsDeleted);
            builder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);
            builder.Entity<SmartContract>().HasQueryFilter(s => !s.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Domain.Common.BaseEntity entity)
                {
                    if (entry.State == EntityState.Modified)
                        entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(ct);
        }
    }
}
