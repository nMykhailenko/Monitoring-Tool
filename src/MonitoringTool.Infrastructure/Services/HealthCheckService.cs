using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Interfaces.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Interfaces.Services.Communication.Http;
using MonitoringTool.Application.Models.ResponseModels.Errors;
using MonitoringTool.Application.Models.ResponseModels.HealthCheck;
using MonitoringTool.Domain.Entities;
using OneOf;

namespace MonitoringTool.Infrastructure.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IConnectedClientRepository _connectedClientRepository;
        private readonly IHttpCommunicationService _httpCommunicationService;
 
        public HealthCheckService(
            IConnectedClientRepository connectedClientRepository, 
            IHttpCommunicationService httpCommunicationService)
        {
            _connectedClientRepository = connectedClientRepository;
            _httpCommunicationService = httpCommunicationService;
        }

        public async Task CheckAsync(CancellationToken cancellationToken)
        {
            var connectedClients = await _connectedClientRepository.GetActiveAsync(cancellationToken);
            
            // TODO refactor for Parallel foreach 
            foreach (var connectedClient in connectedClients)
            {
                var clientServicesHealthCheckTasks = connectedClient.ConnectedServices
                    .Select(x => CheckService(x, cancellationToken));
                var responses = await Task.WhenAll(clientServicesHealthCheckTasks);
                foreach (var response in responses)
                {
                    // TODO send notification if error. send success data to Elastic.
                    response.Match(
                        success => true,
                        error => false);
                }
            }
        }

        private Task<OneOf<HealthCheckResponse, ErrorResponse>> CheckService(
            ConnectedService connectedService,
            CancellationToken cancellationToken)
        {
            var url = $"{connectedService.BaseUrl}/health";
            return _httpCommunicationService.SendAsync<HealthCheckResponse>(url, HttpMethod.Get, cancellationToken);
        }
    }
}