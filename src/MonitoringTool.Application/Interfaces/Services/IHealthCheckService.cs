using System.Threading;
using System.Threading.Tasks;

namespace MonitoringTool.Application.Interfaces.Services
{
    public interface IHealthCheckService
    {
        Task CheckAsync(CancellationToken cancellationToken);
    }
}