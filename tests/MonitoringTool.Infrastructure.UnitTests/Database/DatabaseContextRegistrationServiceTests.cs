using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Domain.Enums;
using MonitoringTool.Infrastructure.Database;
using MonitoringTool.Infrastructure.UnitTests.Constants;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Database
{
    public class DatabaseContextRegistrationServiceTests
    {
        private const string ConenctionString = "some connection string";

        [Theory]
        [InlineData(DatabaseType.PostgreSql, DatabaseConstants.PostgreSqlProviderName)]
        [InlineData(DatabaseType.SqlServer, DatabaseConstants.SqlServerProviderName)]
        public void
            DatabaseContextRegistrationService_ShouldAdd_ToServiceCollection_ValidTypeOfDatabase(
                DatabaseType databaseType,
                string expectedProviderName)
        {
            // arrange
            IServiceCollection serviceCollection = new ServiceCollection();
            var databaseContextRegistrationService = new DatabaseContextRegistrationFactory()
                .Create(databaseType);
            
            // act
            serviceCollection = databaseContextRegistrationService
                .Register(ConenctionString, serviceCollection);
            
            // assert
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            context.Should().NotBeNull();
            context.Database.Should().NotBeNull();
            context.Database.ProviderName.Should().NotBeNull();
            context.Database.ProviderName.Should().Equals(expectedProviderName);
        }
    }
}