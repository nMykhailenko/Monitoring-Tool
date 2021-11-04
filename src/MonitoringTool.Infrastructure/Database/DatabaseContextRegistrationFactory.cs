using System.Collections.Generic;
using MonitoringTool.Application.Interfaces.Database;
using MonitoringTool.Domain.Enums;

namespace MonitoringTool.Infrastructure.Database
{
    public class DatabaseContextRegistrationFactory : IDatabaseContextRegistrationFactory
    {
        private readonly Dictionary<DatabaseType, IDatabaseContextRegistrationService> _factories;

        public DatabaseContextRegistrationFactory()
        {
            _factories = new ()
            {
                { DatabaseType.PostgreSql, new PostgreSqlContextRegistrationService()},
                { DatabaseType.SqlServer , new SqlServerContextRegistrationService()}
            };
        }

        public IDatabaseContextRegistrationService Create(DatabaseType databaseType)
            => _factories[databaseType];
    }
}