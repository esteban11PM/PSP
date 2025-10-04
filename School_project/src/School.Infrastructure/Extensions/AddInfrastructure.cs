using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace School.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("SqlServer")!;
        services.AddDbContext<Persistence.SchoolDbContext>(opt =>
            opt.UseSqlServer(cs, b => b.MigrationsAssembly(typeof(Persistence.SchoolDbContext).Assembly.FullName)));

        // Aquí registras Repositorios y UoW si los usas:
        // services.AddScoped<IStudentRepository, StudentRepository>();
        // services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
