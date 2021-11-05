using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Infrastructure.Services
{
    public class ConnectedClientService : IConnectedClientService
    {
        private readonly IMapper _mapper;
        private readonly IConnectedClientRepository _connectedClientRepository;

        public ConnectedClientService(
            IMapper mapper,
            IConnectedClientRepository connectedClientRepository)
        {
            _mapper = mapper;
            _connectedClientRepository = connectedClientRepository;
        }

        public async Task<ConnectedClientResponse> AddAsync(CreateConnectedClientRequest request, CancellationToken cancellationToken)
        {
            var connectedClient = _mapper.Map<ConnectedClient>(request);

            var addedConnectedClient = await _connectedClientRepository.AddAsync(connectedClient, cancellationToken);
            var response = _mapper.Map<ConnectedClientResponse>(addedConnectedClient);

            return response;
        }
    }
}