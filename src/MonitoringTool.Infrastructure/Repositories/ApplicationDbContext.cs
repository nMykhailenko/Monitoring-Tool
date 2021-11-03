using Microsoft.EntityFrameworkCore;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Infrastructure.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ConnectedClient> ConnectedClients { get; set; }
    }
}