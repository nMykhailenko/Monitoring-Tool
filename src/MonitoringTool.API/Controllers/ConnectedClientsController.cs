using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;

namespace MonitoringTool.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/connectedClients")]
    public class ConnectedClientsController : ControllerBase
    {
        private readonly IConnectedClientService _connectedClientService;

        public ConnectedClientsController(IConnectedClientService connectedClientService)
        {
            _connectedClientService = connectedClientService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(
            [FromBody] CreateConnectedClientRequest request,
            CancellationToken cancellationToken)
        {
            await _connectedClientService.AddAsync(request, cancellationToken);

            return Ok();
        }
    }
}