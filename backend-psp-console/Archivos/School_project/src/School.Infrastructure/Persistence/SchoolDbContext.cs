using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Persistence.Seed;

namespace School.Infrastructure.Persistence;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Period> Periods => Set<Period>();
    public DbSet<SubjectOffering> SubjectOfferings => Set<SubjectOffering>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<AssessmentType> AssessmentTypes => Set<AssessmentType>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);

        // 👇 semilla inicial
        modelBuilder.SeedInitialData();

        base.OnModelCreating(modelBuilder);
    }
}
