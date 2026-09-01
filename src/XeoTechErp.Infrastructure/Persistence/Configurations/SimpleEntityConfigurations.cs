using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Configurations;

public sealed class SimpleEntityConfigurations :
    IEntityTypeConfiguration<Employee>,
    IEntityTypeConfiguration<AuditLogEntry>,
    IEntityTypeConfiguration<Notification>,
    IEntityTypeConfiguration<Activity>,
    IEntityTypeConfiguration<AppConfig>
{
    public void Configure(EntityTypeBuilder<Employee> builder) => builder.HasKey(x => x.Id);
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder) => builder.HasKey(x => x.Id);
    public void Configure(EntityTypeBuilder<Notification> builder) => builder.HasKey(x => x.Id);
    public void Configure(EntityTypeBuilder<Activity> builder) => builder.HasKey(x => x.Id);
    public void Configure(EntityTypeBuilder<AppConfig> builder) => builder.HasKey(x => x.Id);
}
