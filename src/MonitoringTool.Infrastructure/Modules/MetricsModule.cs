using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Common.Interfaces.Configuration;

namespace MonitoringTool.Infrastructure.Modules
{
    public class MetricsModule : IInjectModule
    {
        private readonly IConfiguration _configuration;

        public MetricsModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceCollection Load(IServiceCollection services)
        {

            return services;
        }
    }
}