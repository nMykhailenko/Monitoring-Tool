using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.Errors;
using MonitoringTool.Domain.Entities;
using OneOf;

namespace MonitoringTool.Infrastructure.Services
{
    public class ConnectedClientService : IConnectedClientService
    {
        private static readonly ParallelOptions ParallelOptions = new ()
        {
            MaxDegreeOfParallelism = 5
        };
        
        private readonly IMapper _mapper;
        private readonly IConnectedClientRepository _connectedClientRepository;

        public ConnectedClientService(
            IMapper mapper,
            IConnectedClientRepository connectedClientRepository)
        {
            _mapper = mapper;
            _connectedClientRepository = connectedClientRepository;
        }

        public async Task<IEnumerable<ConnectedClientResponse>>  GetActiveAsync(CancellationToken cancellationToken)
        {
            var connectedClients = await _connectedClientRepository.GetActiveAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ConnectedClientResponse>>(connectedClients);
        }

        public async Task<OneOf<ConnectedClientResponse, EntityIsAlreadyExists>> AddAsync(
            CreateConnectedClientRequest request, 
            CancellationToken cancellationToken)
        {
            var connectedClient = _mapper.Map<ConnectedClient>(request);
           
            var currentConnectedClient = await _connectedClientRepository.GetByNameAsync(
                connectedClient.Name,
                cancellationToken);
            if (currentConnectedClient is null)
            {
                var addedConnectedClient = await _connectedClientRepository.AddAsync(connectedClient, cancellationToken);
                return  _mapper.Map<ConnectedClientResponse>(addedConnectedClient);
            }

            var errorMessage = $"The Connected Client with name: {request.Name} is already exists." +
                               $"If you want to add Connected Service for this client you should use another endpoint.";
            return new EntityIsAlreadyExists(errorMessage);
        }

        private async Task<IEnumerable<ConnectedServiceResponse>> AddConnectedServicesToClientAsync(
            ConnectedClient currentConnectedClient,
            IEnumerable<ConnectedService> connectedServicesToAdd)
        {
            var result = new List<ConnectedServiceResponse>();
            await Parallel.ForEachAsync(
                connectedServicesToAdd,
                ParallelOptions,
                async (connectedService, token) =>
                {
                    var isConnectedServiceExists = IsConnectedServiceExists(
                        connectedService.Name,
                        currentConnectedClient.ConnectedServices);
                    
                    if (!isConnectedServiceExists)
                    {
                        connectedService.ConnectedClientId = currentConnectedClient.Id;
                        var addedConnectedService = await _connectedClientRepository
                            .AddConnectedServiceAsync(connectedService, token);
                        
                        result.Add(_mapper.Map<ConnectedServiceResponse>(addedConnectedService));
                    }
                }
            );

            return result;
        }
        
        private bool IsConnectedServiceExists(
            string name, 
            IEnumerable<ConnectedService> currentConnectedServices)
        {
            return currentConnectedServices.FirstOrDefault(x
                => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }
    }
}