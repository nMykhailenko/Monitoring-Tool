using OneOf;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.Errors;

namespace MonitoringTool.Application.Interfaces.Services
{
    public interface IConnectedClientService
    {
        Task<OneOf<ConnectedClientResponse, EntityNotFoundResponse>> GetByNameAsync(
            string name,
            CancellationToken cancellationToken);
        Task<IEnumerable<ConnectedClientResponse>> GetActiveAsync(CancellationToken cancellationToken);
        Task<OneOf<ConnectedClientResponse, EntityIsAlreadyExists>>  AddAsync(
            CreateConnectedClientRequest request, 
            CancellationToken cancellationToken);

        Task<OneOf<ConnectedServiceResponse, EntityNotFoundResponse, EntityIsAlreadyExists>> AddConnectedServiceToClientAsync(
            string connectedClientName,
            CreateConnectedServiceRequest connectedServiceRequest,
            CancellationToken cancellationToken);
        Task<OneOf<IEnumerable<ConnectedServiceResponse>, EntityNotFoundResponse>> AddConnectedServicesToClientAsync(
            string connectedClientName,
            IEnumerable<CreateConnectedServiceRequest> connectedServicesRequests,
            CancellationToken cancellationToken);
    }
}