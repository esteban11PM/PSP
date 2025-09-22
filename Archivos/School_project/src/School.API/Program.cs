using School.Application;
using School.Infrastructure.Extensions;
using FluentValidation;
using School.Application.Services.Interfaces;
using School.Application.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application & Validators
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssembly(typeof(School.Application.AssemblyRef).Assembly);

// Infra (DbContext, repos, UoW)
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ISubjectOfferingService, SubjectOfferingService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IGradeService, GradeService>();

var app = builder.Build();

const string FrontCors = "FrontCors";
app.UseCors(p => p
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:5500","http://127.0.0.1:5500","http://localhost:5173"));
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
