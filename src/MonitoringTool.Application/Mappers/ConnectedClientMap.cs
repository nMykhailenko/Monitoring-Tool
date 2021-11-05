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
            CreateMap<CreateConnectedClientRequest, ConnectedClient>(MemberList.Source)
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.ConnectedServices, opt => opt.MapFrom(src => src.ConnectedServices));
            CreateMap<CreateConnectedServiceRequest, ConnectedService>(MemberList.Source)
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<ConnectedClient, ConnectedClientResponse>();
            CreateMap<ConnectedService, ConnectedServiceResponse>();
        }
    }
}