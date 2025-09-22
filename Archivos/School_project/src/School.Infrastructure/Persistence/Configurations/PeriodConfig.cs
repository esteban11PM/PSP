using School.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace School.Infrastructure.Persistence.Configurations;

public class PeriodConfig : IEntityTypeConfiguration<Period>
{
    public void Configure(EntityTypeBuilder<Period> b)
    {
        b.Property(x => x.Name).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.Status).HasMaxLength(10).HasDefaultValue("Open");
    }
}
