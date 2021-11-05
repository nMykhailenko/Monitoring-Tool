using AutoMapper;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Application.Mappers
{
    public class ConnectedClientMap : Profile
    {
        public ConnectedClientMap()
        {
            CreateMap<CreateConnectedClientRequest, ConnectedClient>();
            CreateMap<CreateConnectedServiceRequest, ConnectedService>();

            CreateMap<ConnectedClient, ConnectedClientResponse>();
            CreateMap<ConnectedService, ConnectedServiceResponse>();
        }
    }
}