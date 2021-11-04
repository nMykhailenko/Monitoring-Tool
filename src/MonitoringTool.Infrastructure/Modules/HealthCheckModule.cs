using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Common.Interfaces.Configuration;
using MonitoringTool.Infrastructure.Services;

namespace MonitoringTool.Infrastructure.Modules
{
    public class HealthCheckModule : IInjectModule
    {
        public IServiceCollection Load(IServiceCollection services)
        {
            services.AddScoped<IHealthCheckService, HealthCheckService>();
            
            return services;
        }
    }
}