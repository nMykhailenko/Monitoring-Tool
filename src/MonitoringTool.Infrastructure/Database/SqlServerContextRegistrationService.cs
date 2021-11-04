using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Database;
using MonitoringTool.Infrastructure.Repositories;

namespace MonitoringTool.Infrastructure.Database
{
    public class SqlServerContextRegistrationService : IDatabaseContextRegistrationService
    {
        public IServiceCollection Register(string connectionString, IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, _ => _.EnableRetryOnFailure()));

            return services;
        }
    }
}