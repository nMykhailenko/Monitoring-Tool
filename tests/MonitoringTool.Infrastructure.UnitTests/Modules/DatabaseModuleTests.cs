using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Application.Interfaces.Database;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Common.Extensions;
using MonitoringTool.Domain.Enums;
using MonitoringTool.Infrastructure.Database;
using MonitoringTool.Infrastructure.Modules;
using MonitoringTool.Infrastructure.UnitTests.Constants;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Modules
{
    public class DatabaseModuleTests
    {
        private const string ConnectionString = "some connection string";
        
        [Theory]
        [InlineData(DatabaseType.PostgreSql, DatabaseConstants.PostgreSqlProviderName)]
        [InlineData(DatabaseType.SqlServer, DatabaseConstants.SqlServerProviderName)]
        public void DatabaseModule_ShouldLoad_AllNecessaryDI(
            DatabaseType databaseType,
            string expectedProviderName)
        {
            // arrange
            var configuration = GetConfiguration(databaseType);
            IServiceCollection serviceCollection = new ServiceCollection();

            // act
            serviceCollection = serviceCollection.RegisterModule(new DatabaseModule(configuration));

            // assert
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = serviceProvider.GetService<ApplicationDbContext>();
            var connectedClientRepository = serviceProvider.GetService<IConnectedClientRepository>();
            var databaseContextRegistrationFactory =
                serviceProvider.GetService<IDatabaseContextRegistrationFactory>();

            connectedClientRepository.Should().NotBeNull();
            databaseContextRegistrationFactory.Should().NotBeNull();
            context.Should().NotBeNull();
            context!.Database.Should().NotBeNull();
            context.Database.ProviderName.Should().NotBeNull();
            context.Database.ProviderName.Should().Equals(expectedProviderName);
        }

        private static IConfiguration GetConfiguration(DatabaseType databaseType)
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"DatabaseOptions:DefaultConnection", ConnectionString},
                {"DatabaseOptions:DatabaseType", $"{databaseType}"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return configuration;
        }
    }
}