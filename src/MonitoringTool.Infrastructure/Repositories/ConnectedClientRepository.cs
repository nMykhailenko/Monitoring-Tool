using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MonitoringTool.Application.Interfaces.Repositories;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Infrastructure.Repositories
{
    public class ConnectedClientRepository : IConnectedClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ConnectedClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConnectedClient>> GetActiveAsync(CancellationToken cancellationToken)
        { 
            var result = await _context.ConnectedClients
                .Include(x => x.ConnectedServices)
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<IEnumerable<ConnectedClient>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _context.ConnectedClients
                .Include(x => x.ConnectedServices)
                .Where(x => !x.IsActive)
                .ToListAsync(cancellationToken);

            return result;        }
    }
}