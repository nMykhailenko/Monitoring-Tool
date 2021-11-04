using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Database;

namespace MonitoringTool.Infrastructure.Database
{
    public class PostgreSqlContextRegistrationService : IDatabaseContextRegistrationService
    {
        public IServiceCollection Register(string connectionString, IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, _ => _.EnableRetryOnFailure()));

            return services;
        }
    }
}