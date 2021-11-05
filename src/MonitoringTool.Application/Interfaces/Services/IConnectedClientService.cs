using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;

namespace MonitoringTool.Application.Interfaces.Services
{
    public interface IConnectedClientService
    {
        Task<ConnectedClientResponse>  AddAsync(CreateConnectedClientRequest request, CancellationToken cancellationToken);
    }
}