using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Interfaces.Database.Repositories;
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
        private static readonly ParallelOptions ParallelOptions = new ()
        {
            MaxDegreeOfParallelism = 5
        };

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
            var connectedClientList = connectedClients.ToList();
            
            await Parallel.ForEachAsync(
                connectedClientList,
                ParallelOptions,
                async (connectedClient, token) =>
                {
                    await CheckServicesAsync(connectedClient.ConnectedServices);
                }
            );
        }

        private async Task CheckServicesAsync(
            IEnumerable<ConnectedService> connectedServices)
        {
            await Parallel.ForEachAsync(
                connectedServices,
                ParallelOptions,
                async (connectedService, token) =>
                {
                    var checkServiceResponse =
                        await CheckServiceAsync(connectedService, token);
                    
                    // TODO send notification if error. send success data to Elastic.
                    checkServiceResponse.Match(
                        success => true,
                        error => false);
                });
        }
        
        private Task<OneOf<HealthCheckResponse, ErrorResponse>> CheckServiceAsync(
            ConnectedService connectedService,
            CancellationToken cancellationToken)
        {
            var url = $"{connectedService.BaseUrl}/health";
            return _httpCommunicationService.SendAsync<HealthCheckResponse>(url, HttpMethod.Get, cancellationToken);
        }
    }
}