using School.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace School.Infrastructure.Persistence.Configurations;

public class TeacherConfig : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> b)
    {
        b.Property(x => x.DocumentNumber).HasMaxLength(30).IsRequired();
        b.HasIndex(x => x.DocumentNumber).IsUnique();
        b.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(80).IsRequired();
        b.Property(x => x.Email).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Active");
    }
}
