using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace School.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Periods",
                columns: new[] { "Id", "EndDate", "Name", "StartDate", "Status" },
                values: new object[] { 1, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "2025-P1", new DateTime(2025, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Open" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "DocumentNumber", "Email", "FirstName", "LastName", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "S-2001", "esteban.palomar@colegio.test", "Esteban", "Palomar", "Active", null },
                    { 2, null, new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "S-2002", "maria.gomez@colegio.test", "María", "Gómez", "Active", null },
                    { 3, null, new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "S-2003", "luis.ortega@colegio.test", "Luis", "Ortega", "Active", null },
                    { 4, null, new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "S-2004", "sara.bermudez@colegio.test", "Sara", "Bermúdez", "Active", null },
                    { 5, null, new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "S-2005", "ivan.rojas@colegio.test", "Iván", "Rojas", "Active", null }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Code", "Name", "WeeklyHours" },
                values: new object[,]
                {
                    { 1, "MAT101", "Matemáticas", (byte)4 },
                    { 2, "LEN201", "Lengua", (byte)3 },
                    { 3, "CIE301", "Ciencias", (byte)3 }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "CreatedAt", "DocumentNumber", "Email", "FirstName", "LastName", "Specialty", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), "T-1001", "ana.ruiz@colegio.test", "Ana", "Ruiz", "Matemáticas", "Active", null },
                    { 2, new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), "T-1002", "carlos.mejia@colegio.test", "Carlos", "Mejía", "Lengua", "Active", null }
                });

            migrationBuilder.InsertData(
                table: "SubjectOfferings",
                columns: new[] { "Id", "IsClosed", "PeriodId", "SubjectId", "TeacherId" },
                values: new object[,]
                {
                    { 1, false, 1, 1, 1 },
                    { 2, false, 1, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "AssessmentTypes",
                columns: new[] { "Id", "Name", "SubjectOfferingId", "Weight" },
                values: new object[,]
                {
                    { 1, "Parcial", 1, (byte)40 },
                    { 2, "Taller", 1, (byte)30 },
                    { 3, "Examen", 1, (byte)30 },
                    { 4, "Parcial", 2, (byte)40 },
                    { 5, "Taller", 2, (byte)30 },
                    { 6, "Examen", 2, (byte)30 }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "EnrolledAt", "FinalAverage", "StudentId", "SubjectOfferingId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Utc), 4.00m, 1, 1 },
                    { 2, new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Utc), 3.50m, 2, 1 },
                    { 3, new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Utc), 4.44m, 3, 1 },
                    { 4, new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Utc), 4.18m, 1, 2 },
                    { 5, new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Utc), 3.39m, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "AssessmentTypeId", "EnrollmentId", "GradedAt", "Score" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.00m },
                    { 2, 2, 1, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.20m },
                    { 3, 3, 1, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 3.80m },
                    { 4, 1, 2, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 3.50m },
                    { 5, 2, 2, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.00m },
                    { 6, 3, 2, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 3.00m },
                    { 7, 1, 3, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.50m },
                    { 8, 2, 3, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.80m },
                    { 9, 3, 3, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.00m },
                    { 10, 4, 4, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.30m },
                    { 11, 5, 4, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.00m },
                    { 12, 6, 4, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 4.20m },
                    { 13, 4, 5, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 3.00m },
                    { 14, 5, 5, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 3.50m },
                    { 15, 6, 5, new DateTime(2025, 4, 15, 15, 30, 0, 0, DateTimeKind.Utc), 3.80m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AssessmentTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AssessmentTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AssessmentTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AssessmentTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AssessmentTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AssessmentTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubjectOfferings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubjectOfferings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Periods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
