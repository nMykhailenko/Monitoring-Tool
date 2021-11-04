using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Database;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Common.Interfaces.Configuration;
using MonitoringTool.Common.Options;
using MonitoringTool.Infrastructure.Database;

namespace MonitoringTool.Infrastructure.Modules
{
    public class DatabaseModule : IInjectModule
    {
        private readonly IConfiguration _configuration;

        public DatabaseModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceCollection Load(IServiceCollection services)
        {
            var databaseOptions = _configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
            services.Configure<DatabaseOptions>(x 
                => _configuration.GetSection(nameof(DatabaseOptions)).Bind(x));

            services.AddScoped<IConnectedClientRepository, IConnectedClientRepository>();
            services.AddScoped<IDatabaseContextRegistrationFactory, DatabaseContextRegistrationFactory>();

            var serviceProvider = services.BuildServiceProvider();
            var databaseContextRegistrationFactory = 
                serviceProvider.GetRequiredService<IDatabaseContextRegistrationFactory>();

            services = databaseContextRegistrationFactory
                .Create(databaseOptions.DatabaseType)
                .Register(databaseOptions.DefaultConnection, services);
            
            return services;
        }
    }
}