using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;

namespace MonitoringTool.Application.Interfaces.Services
{
    public interface IConnectedClientService
    {
        Task<IEnumerable<ConnectedClientResponse>> GetActiveAsync(CancellationToken cancellationToken);
        Task  AddAsync(CreateConnectedClientRequest request, CancellationToken cancellationToken);
    }
}