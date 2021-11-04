using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MonitoringTool.Domain.Enums;
using MonitoringTool.Infrastructure.Database;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Database
{
    public class DatabaseContextRegistrationFactoryTests
    {
        [Theory]
        [InlineData(DatabaseType.PostgreSql, typeof(PostgreSqlContextRegistrationService))]
        [InlineData(DatabaseType.SqlServer, typeof(SqlServerContextRegistrationService))]
        public void
            DatabaseContextRegistrationFactory_ShouldCreate_ValidDatabaseContextRegistrationServiceInstance(
                DatabaseType databaseType,
                Type expectedDatabaseContextRegistrationService)
        {
            // arrange & act
            var actualDatabaseContextRegistrationService = new DatabaseContextRegistrationFactory()
                .Create(databaseType);

            // assert
            actualDatabaseContextRegistrationService.Should().BeOfType(expectedDatabaseContextRegistrationService);
        }

        [Fact]
        public void
            DatabaseContextRegistrationFactory_ShouldHave_DatabaseContextRegistrationServiceInstance_ForEach_DatabaseType()
        {
            // arrange
            var databaseTypes = Enum.GetValues<DatabaseType>().ToList();
            
            // act & assert
            foreach (var databaseType in databaseTypes )
            {
                var subject = new DatabaseContextRegistrationFactory();
                subject.Invoking(_ => _.Create(databaseType))
                    .Should().NotThrow<KeyNotFoundException>();
            }
        }
    }
}