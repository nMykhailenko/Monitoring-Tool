using Microsoft.EntityFrameworkCore;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<ConnectedClient> ConnectedClients { get; set; }
    }
}