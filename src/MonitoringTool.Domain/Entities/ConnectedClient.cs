using System.Collections.Generic;

namespace MonitoringTool.Domain.Entities
{
    public class ConnectedClient
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        
        public ICollection<ConnectedService> ConnectedServices { get; set; } = null!;
    }
}