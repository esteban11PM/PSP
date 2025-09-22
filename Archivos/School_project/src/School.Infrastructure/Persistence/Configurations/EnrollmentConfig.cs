using School.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace School.Infrastructure.Persistence.Configurations;

public class EnrollmentConfig : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> b)
    {
        b.HasIndex(x => new { x.StudentId, x.SubjectOfferingId }).IsUnique();

        b.HasOne(x => x.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.SubjectOffering)
            .WithMany(o => o.Enrollments)
            .HasForeignKey(x => x.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
