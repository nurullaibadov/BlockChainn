using Blockchain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blockchain.Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FromAddress).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ToAddress).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Amount).HasPrecision(36, 18);
            builder.Property(x => x.GasPrice).HasPrecision(36, 18);
            builder.Property(x => x.GasUsed).HasPrecision(36, 18);
            builder.Property(x => x.Fee).HasPrecision(36, 18);
            builder.Property(x => x.TxHash).HasMaxLength(100);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FromWallet)
                .WithMany(x => x.SentTransactions)
                .HasForeignKey(x => x.FromWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ToWallet)
                .WithMany(x => x.ReceivedTransactions)
                .HasForeignKey(x => x.ToWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.TxHash);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Status);
        }
    }
}
