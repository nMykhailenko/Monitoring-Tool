using System.Collections.Generic;
using MonitoringTool.Domain.Enums;

namespace MonitoringTool.Application.Interfaces.Database
{
    public interface IDatabaseContextRegistrationFactory
    {
        IDatabaseContextRegistrationService Create(DatabaseType databaseType);
    }
}