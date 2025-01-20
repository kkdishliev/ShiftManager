using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShiftManager.Application.Interfaces;
using ShiftManager.Application.Mapping;
using ShiftManager.Application.Services;
using ShiftManager.Domain.Interfaces;
using ShiftManager.Infrastructure.Data;
using ShiftManager.Infrastructure.Data.Repositories;
using ShiftManager.Infrastructure.Data.UnitOfWork;
using ShiftManager.Infrastructure.Services;

namespace ShiftManager.Application.Configurations
{
    /// <summary>
    /// Extension methods to configure services for dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ShiftManagerDBContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
 
            services.AddScoped<IQueryService, QueryService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IShiftService, ShiftService>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();

            return services;
        }
    }
}
