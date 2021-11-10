using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Application.Interfaces.Database.Repositories
{
    public interface IConnectedClientRepository
    {
        Task<IEnumerable<ConnectedClient>> GetActiveAsync(CancellationToken cancellationToken);
        Task<IEnumerable<ConnectedClient>> GetAllAsync(CancellationToken cancellationToken);
        Task<ConnectedClient?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<ConnectedClient> AddAsync(ConnectedClient connectedClient, CancellationToken cancellationToken);
        Task<ConnectedService> AddConnectedServiceAsync(
            ConnectedService connectedService,
            CancellationToken cancellationToken);
    }
}