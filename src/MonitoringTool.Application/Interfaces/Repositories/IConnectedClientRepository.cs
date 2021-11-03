using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Application.Interfaces.Repositories
{
    public interface IConnectedClientRepository
    {
        Task<IEnumerable<ConnectedClient>> GetActiveAsync(CancellationToken cancellationToken);
        Task<IEnumerable<ConnectedClient>> GetAllAsync(CancellationToken cancellationToken);
    }
}