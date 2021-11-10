using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Domain.Entities;
using OneOf;

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

        public async Task<IEnumerable<ConnectedClientResponse>>  GetActiveAsync(CancellationToken cancellationToken)
        {
            var connectedClients = await _connectedClientRepository.GetActiveAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ConnectedClientResponse>>(connectedClients);
        }

        public async Task AddAsync(CreateConnectedClientRequest request, CancellationToken cancellationToken)
        {
            var connectedClient = _mapper.Map<ConnectedClient>(request);

            await _connectedClientRepository.AddAsync(connectedClient, cancellationToken);
        }
    }
}