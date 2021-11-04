using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Services.Communication.Http;
using MonitoringTool.Common.Interfaces.Configuration;
using MonitoringTool.Infrastructure.Services.Communication.Http;

namespace MonitoringTool.Infrastructure.Modules
{
    public class CommunicationModule : IInjectModule
    {
        public IServiceCollection Load(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IHttpCommunicationService, HttpCommunicationService>();

            return services;
        }
    }
}