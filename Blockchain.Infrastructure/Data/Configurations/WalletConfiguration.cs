using Blockchain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blockchain.Infrastructure.Data.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EncryptedPrivateKey).IsRequired();
            builder.Property(x => x.PublicKey).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Label).HasMaxLength(100);
            builder.Property(x => x.Balance).HasPrecision(36, 18);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Wallets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Address);
            builder.HasIndex(x => new { x.UserId, x.Network });
        }
    }
}
