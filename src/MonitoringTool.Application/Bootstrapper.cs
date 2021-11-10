using Microsoft.Extensions.DependencyInjection;

namespace MonitoringTool.Application
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Bootstrapper));
            
            return services;
        }
    }
}