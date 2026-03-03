using Blockchain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blockchain.Infrastructure.Data.Configurations
{
    public class SmartContractConfiguration : IEntityTypeConfiguration<SmartContract>
    {
        public void Configure(EntityTypeBuilder<SmartContract> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ContractAddress).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Abi).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Interactions)
                .WithOne(x => x.Contract)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ContractAddress);
        }
    }
}
