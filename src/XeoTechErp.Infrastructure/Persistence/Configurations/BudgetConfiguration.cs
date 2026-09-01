using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Configurations;

public sealed class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.MonthlyAmount).HasPrecision(18, 2); builder.HasIndex(x => x.Category).IsUnique(); }
}
