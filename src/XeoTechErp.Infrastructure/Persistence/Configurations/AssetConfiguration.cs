using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Configurations;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Cost).HasPrecision(18, 2); builder.Property(x => x.Salvage).HasPrecision(18, 2); }
}
