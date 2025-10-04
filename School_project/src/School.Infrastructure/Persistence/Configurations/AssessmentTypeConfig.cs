using School.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace School.Infrastructure.Persistence.Configurations;

public class AssessmentTypeConfig : IEntityTypeConfiguration<AssessmentType>
{
    public void Configure(EntityTypeBuilder<AssessmentType> b)
    {
        b.Property(x => x.Name).HasMaxLength(50).IsRequired();
        b.HasIndex(x => new { x.SubjectOfferingId, x.Name }).IsUnique();

        b.HasOne(x => x.SubjectOffering)
            .WithMany(o => o.AssessmentTypes)
            .HasForeignKey(x => x.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
