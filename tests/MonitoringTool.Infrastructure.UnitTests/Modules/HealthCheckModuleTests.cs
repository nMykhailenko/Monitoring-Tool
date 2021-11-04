using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Common.Extensions;
using MonitoringTool.Domain.Enums;
using MonitoringTool.Infrastructure.Modules;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Modules
{
    public class HealthCheckModuleTests
    {
        [Theory]
        [InlineData(DatabaseType.PostgreSql)]
        [InlineData(DatabaseType.SqlServer)]       
        public void HealthCheckModule_ShouldLoad_AllNecessaryDI(DatabaseType databaseType)
        {
            // arrange
            IServiceCollection serviceCollection = new ServiceCollection();
            var configuration = GetConfiguration(databaseType);
            serviceCollection.RegisterModule(new DatabaseModule(configuration));
            serviceCollection.RegisterModule<CommunicationModule>();
            
            // act
            serviceCollection = serviceCollection.RegisterModule<HealthCheckModule>();

            // assert
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var healthCheckService = serviceProvider.GetService<IHealthCheckService>();

            healthCheckService.Should().NotBeNull();
        }
        
        private static IConfiguration GetConfiguration(DatabaseType databaseType)
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"DatabaseOptions:DefaultConnection", "some connection string"},
                {"DatabaseOptions:DatabaseType", $"{databaseType}"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return configuration;
        }
    }
}