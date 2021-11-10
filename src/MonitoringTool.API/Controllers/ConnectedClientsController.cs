using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.Errors;

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
        [ProducesResponseType(typeof(ConnectedClientResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ModelInvalidResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EntityIsAlreadyExists), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddAsync(
            [FromBody] CreateConnectedClientRequest request,
            CancellationToken cancellationToken)
        {
            var result = 
                await _connectedClientService.AddAsync(request, cancellationToken);

            return result.Match<IActionResult>(
                success => Created($"api/connectedClients/{success.Id}", success),
                entityIsAlreadyExists => Conflict(entityIsAlreadyExists));
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ConnectedClientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveAsync(CancellationToken cancellationToken) =>
            Ok(await _connectedClientService.GetActiveAsync(cancellationToken));
    }
}