using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Configurations;

public sealed class FinanceConfiguration : IEntityTypeConfiguration<Asset>, IEntityTypeConfiguration<Budget>, IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Asset> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Cost).HasPrecision(18, 2); builder.Property(x => x.Salvage).HasPrecision(18, 2); }
    public void Configure(EntityTypeBuilder<Budget> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.MonthlyAmount).HasPrecision(18, 2); builder.HasIndex(x => x.Category).IsUnique(); }
    public void Configure(EntityTypeBuilder<Expense> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Amount).HasPrecision(18, 2); }
}
