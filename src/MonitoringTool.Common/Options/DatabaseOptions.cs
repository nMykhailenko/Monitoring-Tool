using MonitoringTool.Domain.Enums;

namespace MonitoringTool.Common.Options
{
    public class DatabaseOptions
    {
        public string DefaultConnection { get; set; } = null!;
        public DatabaseType DatabaseType { get; set; } = DatabaseType.PostgreSql;
    }
}