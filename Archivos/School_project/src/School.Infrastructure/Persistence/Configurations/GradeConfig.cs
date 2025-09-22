using School.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace School.Infrastructure.Persistence.Configurations;

public class GradeConfig : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> b)
    {
        b.HasIndex(x => new { x.EnrollmentId, x.AssessmentTypeId }).IsUnique();

        b.Property(x => x.Score).HasPrecision(5, 2);

        b.HasOne(x => x.Enrollment)
            .WithMany(e => e.Grades)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.AssessmentType)
            .WithMany(a => a.Grades)
            .HasForeignKey(x => x.AssessmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
