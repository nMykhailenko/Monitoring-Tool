using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MonitoringTool.Domain.Enums;

namespace MonitoringTool.Infrastructure.UnitTests.Utils
{
    public static class ConfigurationUtils
    {
        public static IConfiguration GetConfiguration(DatabaseType databaseType)
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