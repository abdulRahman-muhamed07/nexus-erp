using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.Order).WithOne().HasForeignKey<Invoice>(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
    }
}
