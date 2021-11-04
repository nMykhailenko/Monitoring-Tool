using Microsoft.Extensions.DependencyInjection;

namespace MonitoringTool.Application.Interfaces.Database
{
    public interface IDatabaseContextRegistrationService
    {
        IServiceCollection Register(string connectionString, IServiceCollection services);
    }
}