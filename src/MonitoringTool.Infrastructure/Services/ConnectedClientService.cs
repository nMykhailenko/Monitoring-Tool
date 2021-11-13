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

        public async Task<OneOf<ConnectedClientResponse, EntityNotFoundResponse>> GetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            var connectedClient = await _connectedClientRepository.GetByNameAsync(name, cancellationToken);
            if (connectedClient is null)
            {
                return new EntityNotFoundResponse($"Connected client with name: {name} not found.");
            }

            return _mapper.Map<ConnectedClientResponse>(connectedClient);
        }

        public async Task<OneOf<ConnectedClientResponse, EntityIsAlreadyExists>> AddAsync(
            CreateConnectedClientRequest request, 
            CancellationToken cancellationToken)
        {           
            var currentConnectedClient = await _connectedClientRepository.GetByNameAsync(
                request.Name,
                cancellationToken);
            if (currentConnectedClient is null)
            {
                var connectedClient = _mapper.Map<ConnectedClient>(request);
                var addedConnectedClient = await _connectedClientRepository.AddAsync(connectedClient, cancellationToken);
                
                return  _mapper.Map<ConnectedClientResponse>(addedConnectedClient);
            }

            var errorMessage = $"The Connected Client with name: {request.Name} is already exists." +
                               $"If you want to add Connected Service for this client you should use another endpoint.";
            return new EntityIsAlreadyExists(errorMessage);
        }

        public async Task<OneOf<ConnectedServiceResponse,EntityNotFoundResponse>> AddConnectedServiceToClientAsync(
            string connectedClientName,
            CreateConnectedServiceRequest connectedServiceRequest,
            CancellationToken cancellationToken)
        {
            var connectedServiceRequests = new List<CreateConnectedServiceRequest> { connectedServiceRequest };
            var result = await AddConnectedServicesToClientAsync(
                connectedClientName,
                connectedServiceRequests,
                cancellationToken);

            return result.Match<OneOf<ConnectedServiceResponse, EntityNotFoundResponse>>(
                success => success.First(),
                entityNotFound => entityNotFound);
        }

        
        public async Task<OneOf<IEnumerable<ConnectedServiceResponse>,EntityNotFoundResponse>> AddConnectedServicesToClientAsync(
            string connectedClientName,
            IEnumerable<CreateConnectedServiceRequest> connectedServicesRequests,
            CancellationToken cancellationToken)
        {
            var connectedClientResponse = await GetByNameAsync(connectedClientName, cancellationToken);

            return await connectedClientResponse.Match<Task<OneOf<IEnumerable<ConnectedServiceResponse>,EntityNotFoundResponse>>>(
                async connectedClient =>
                {
                    var connectedServiceResponses =  await AssignConnectedServicesToClientAsync(
                        connectedClient,
                        connectedServicesRequests);
                    return connectedServiceResponses.ToList();
                },
                async entityNotFound =>  entityNotFound);
        }

        private async Task<IEnumerable<ConnectedServiceResponse>> AssignConnectedServicesToClientAsync(
            ConnectedClientResponse connectedClient,
            IEnumerable<CreateConnectedServiceRequest> connectedServicesRequests)
        {
            var connectedServicesToAdd = _mapper.Map<IEnumerable<ConnectedService>>(connectedServicesRequests);
            var result = new List<ConnectedServiceResponse>();
            
            await Parallel.ForEachAsync(
                connectedServicesToAdd,
                ParallelOptions,
                async (connectedService, token) =>
                {
                    var isConnectedServiceExists = IsConnectedServiceExists(
                        connectedService.Name,
                        connectedClient.ConnectedServices);
                    
                    if (!isConnectedServiceExists)
                    {
                        connectedService.ConnectedClientId = connectedClient.Id;
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
            IEnumerable<ConnectedServiceResponse> currentConnectedServices)
        {
            return currentConnectedServices.FirstOrDefault(x
                => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase) &&
                   x.IsActive) != null;
        }
    }
}