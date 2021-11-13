using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Interfaces.Services.Communication.Http;
using MonitoringTool.Domain.Enums;
using MonitoringTool.Infrastructure.Database;
using MonitoringTool.Infrastructure.UnitTests.Utils;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests
{
    public class BootstrapperTests
    {
        [Theory]
        [InlineData(DatabaseType.PostgreSql)]
        [InlineData(DatabaseType.SqlServer)]
        public void AddApplication_Should_Add_All_Needed_Services(DatabaseType databaseType)
        {
            // arrange
            IServiceCollection serviceCollection = new ServiceCollection();
            var configuration = ConfigurationUtils.GetConfiguration(databaseType);
            serviceCollection.AddApplication();
            
            // act
            var actualServiceCollection = serviceCollection.AddInfrastructure(configuration);
            
            // assert
            var serviceProvider = actualServiceCollection.BuildServiceProvider();

            var httpCommunicationService = serviceProvider.GetService<IHttpCommunicationService>();
            httpCommunicationService.Should().NotBeNull();
            
            var context = serviceProvider.GetService<ApplicationDbContext>();
            var connectedClientRepository = serviceProvider.GetService<IConnectedClientRepository>();
            connectedClientRepository.Should().NotBeNull();
            context.Should().NotBeNull();
            
            var healthCheckService = serviceProvider.GetService<IHealthCheckService>();
            healthCheckService.Should().NotBeNull();

            var connectedClientService = serviceProvider.GetService<IConnectedClientService>();
            connectedClientService.Should().NotBeNull();
        }
    }
}