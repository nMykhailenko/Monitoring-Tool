using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;

namespace MonitoringTool.Application.Interfaces.Services
{
    public interface IConnectedClientService
    {
        Task  AddAsync(CreateConnectedClientRequest request, CancellationToken cancellationToken);
    }
}