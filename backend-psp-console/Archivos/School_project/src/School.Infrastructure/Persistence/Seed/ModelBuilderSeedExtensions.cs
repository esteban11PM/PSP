using School.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace School.Infrastructure.Persistence.Seed;

public static class ModelBuilderSeedExtensions
{
    public static void SeedInitialData(this ModelBuilder modelBuilder)
    {
        // ===== Catálogos =====
        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, Code = "MAT101", Name = "Matemáticas", WeeklyHours = 4 },
            new Subject { Id = 2, Code = "LEN201", Name = "Lengua", WeeklyHours = 3 },
            new Subject { Id = 3, Code = "CIE301", Name = "Ciencias", WeeklyHours = 3 }
        );

        modelBuilder.Entity<Period>().HasData(
            new Period
            {
                Id = 1,
                Name = "2025-P1",
                StartDate = new DateTime(2025, 1, 27),
                EndDate = new DateTime(2025, 6, 15),
                Status = "Open"
            }
        );

        modelBuilder.Entity<Teacher>().HasData(
            new Teacher
            {
                Id = 1,
                DocumentNumber = "T-1001",
                FirstName = "Ana",
                LastName = "Ruiz",
                Email = "ana.ruiz@colegio.test",
                Specialty = "Matemáticas",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 10, 12, 0, 0, DateTimeKind.Utc)
            },
            new Teacher
            {
                Id = 2,
                DocumentNumber = "T-1002",
                FirstName = "Carlos",
                LastName = "Mejía",
                Email = "carlos.mejia@colegio.test",
                Specialty = "Lengua",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 10, 12, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                DocumentNumber = "S-2001",
                FirstName = "Esteban",
                LastName = "Palomar",
                Email = "esteban.palomar@colegio.test",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc)
            },
            new Student
            {
                Id = 2,
                DocumentNumber = "S-2002",
                FirstName = "María",
                LastName = "Gómez",
                Email = "maria.gomez@colegio.test",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc)
            },
            new Student
            {
                Id = 3,
                DocumentNumber = "S-2003",
                FirstName = "Luis",
                LastName = "Ortega",
                Email = "luis.ortega@colegio.test",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc)
            },
            new Student
            {
                Id = 4,
                DocumentNumber = "S-2004",
                FirstName = "Sara",
                LastName = "Bermúdez",
                Email = "sara.bermudez@colegio.test",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc)
            },
            new Student
            {
                Id = 5,
                DocumentNumber = "S-2005",
                FirstName = "Iván",
                LastName = "Rojas",
                Email = "ivan.rojas@colegio.test",
                Status = "Active",
                CreatedAt = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc)
            }
        );

        // ===== Materias por Periodo (SubjectOffering) =====
        modelBuilder.Entity<SubjectOffering>().HasData(
            new SubjectOffering { Id = 1, SubjectId = 1, PeriodId = 1, TeacherId = 1, IsClosed = false }, // Matemáticas 2025-P1 (Ana)
            new SubjectOffering { Id = 2, SubjectId = 2, PeriodId = 1, TeacherId = 2, IsClosed = false }  // Lengua 2025-P1 (Carlos)
        );

        // ===== Rubros (suma = 100) =====
        modelBuilder.Entity<AssessmentType>().HasData(
            // Offering 1 (Matemáticas)
            new AssessmentType { Id = 1, SubjectOfferingId = 1, Name = "Parcial", Weight = 40 },
            new AssessmentType { Id = 2, SubjectOfferingId = 1, Name = "Taller", Weight = 30 },
            new AssessmentType { Id = 3, SubjectOfferingId = 1, Name = "Examen", Weight = 30 },
            // Offering 2 (Lengua)
            new AssessmentType { Id = 4, SubjectOfferingId = 2, Name = "Parcial", Weight = 40 },
            new AssessmentType { Id = 5, SubjectOfferingId = 2, Name = "Taller", Weight = 30 },
            new AssessmentType { Id = 6, SubjectOfferingId = 2, Name = "Examen", Weight = 30 }
        );

        // ===== Inscripciones =====
        modelBuilder.Entity<Enrollment>().HasData(
            // Matemáticas
            new Enrollment { Id = 1, SubjectOfferingId = 1, StudentId = 1, EnrolledAt = new DateTime(2025, 2, 1, 14, 0, 0, DateTimeKind.Utc), FinalAverage = 4.00m },
            new Enrollment { Id = 2, SubjectOfferingId = 1, StudentId = 2, EnrolledAt = new DateTime(2025, 2, 1, 14, 0, 0, DateTimeKind.Utc), FinalAverage = 3.50m },
            new Enrollment { Id = 3, SubjectOfferingId = 1, StudentId = 3, EnrolledAt = new DateTime(2025, 2, 1, 14, 0, 0, DateTimeKind.Utc), FinalAverage = 4.44m },

            // Lengua
            new Enrollment { Id = 4, SubjectOfferingId = 2, StudentId = 1, EnrolledAt = new DateTime(2025, 2, 1, 14, 0, 0, DateTimeKind.Utc), FinalAverage = 4.18m },
            new Enrollment { Id = 5, SubjectOfferingId = 2, StudentId = 2, EnrolledAt = new DateTime(2025, 2, 1, 14, 0, 0, DateTimeKind.Utc), FinalAverage = 3.39m }
        );

        // ===== Notas (Grades) =====
        var graded = new DateTime(2025, 4, 15, 15, 30, 0, DateTimeKind.Utc);

        // Matemáticas (enrollment 1..3 ; assessments 1..3)
        modelBuilder.Entity<Grade>().HasData(
            // Esteban (final 4.00): 4.0, 4.2, 3.8
            new Grade { Id = 1, EnrollmentId = 1, AssessmentTypeId = 1, Score = 4.00m, GradedAt = graded },
            new Grade { Id = 2, EnrollmentId = 1, AssessmentTypeId = 2, Score = 4.20m, GradedAt = graded },
            new Grade { Id = 3, EnrollmentId = 1, AssessmentTypeId = 3, Score = 3.80m, GradedAt = graded },

            // María (final 3.50): 3.5, 4.0, 3.0
            new Grade { Id = 4, EnrollmentId = 2, AssessmentTypeId = 1, Score = 3.50m, GradedAt = graded },
            new Grade { Id = 5, EnrollmentId = 2, AssessmentTypeId = 2, Score = 4.00m, GradedAt = graded },
            new Grade { Id = 6, EnrollmentId = 2, AssessmentTypeId = 3, Score = 3.00m, GradedAt = graded },

            // Luis (final 4.44): 4.5, 4.8, 4.0
            new Grade { Id = 7, EnrollmentId = 3, AssessmentTypeId = 1, Score = 4.50m, GradedAt = graded },
            new Grade { Id = 8, EnrollmentId = 3, AssessmentTypeId = 2, Score = 4.80m, GradedAt = graded },
            new Grade { Id = 9, EnrollmentId = 3, AssessmentTypeId = 3, Score = 4.00m, GradedAt = graded },

            // Lengua (enrollment 4..5 ; assessments 4..6)
            // Esteban (final 4.18): 4.3, 4.0, 4.2
            new Grade { Id = 10, EnrollmentId = 4, AssessmentTypeId = 4, Score = 4.30m, GradedAt = graded },
            new Grade { Id = 11, EnrollmentId = 4, AssessmentTypeId = 5, Score = 4.00m, GradedAt = graded },
            new Grade { Id = 12, EnrollmentId = 4, AssessmentTypeId = 6, Score = 4.20m, GradedAt = graded },

            // María (final 3.39): 3.0, 3.5, 3.8
            new Grade { Id = 13, EnrollmentId = 5, AssessmentTypeId = 4, Score = 3.00m, GradedAt = graded },
            new Grade { Id = 14, EnrollmentId = 5, AssessmentTypeId = 5, Score = 3.50m, GradedAt = graded },
            new Grade { Id = 15, EnrollmentId = 5, AssessmentTypeId = 6, Score = 3.80m, GradedAt = graded }
        );
    }
}
