using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Infrastructure.Database.Repositories
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

        public Task<ConnectedClient?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return _context.ConnectedClients
                .Include(x => x.ConnectedServices)
                .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<ConnectedClient> AddAsync(ConnectedClient connectedClient, CancellationToken cancellationToken)
        {
            await _context.ConnectedClients.AddAsync(connectedClient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return connectedClient;
        }

        public async Task<ConnectedService> AddConnectedServiceAsync(
            ConnectedService connectedService, 
            CancellationToken cancellationToken)
        {
            await _context.ConnectedService.AddAsync(connectedService, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return connectedService;
        }
    }
}