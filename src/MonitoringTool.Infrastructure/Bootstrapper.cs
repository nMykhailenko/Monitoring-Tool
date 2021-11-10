using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Common.Extensions;
using MonitoringTool.Infrastructure.Modules;
using MonitoringTool.Infrastructure.Services;

namespace MonitoringTool.Infrastructure
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IConnectedClientService, ConnectedClientService>();
            
            services.RegisterModule<CommunicationModule>();
            services.RegisterModule<HealthCheckModule>();
            services.RegisterModule(new DatabaseModule(configuration));
            
            return services;
        }
    }
}