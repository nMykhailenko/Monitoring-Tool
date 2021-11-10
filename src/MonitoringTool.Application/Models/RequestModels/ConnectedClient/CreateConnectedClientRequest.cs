using System.Collections.Generic;

namespace MonitoringTool.Application.Models.RequestModels.ConnectedClient
{
    public record CreateConnectedClientRequest
    {
        public string Name { get; init; } = null!;
        public List<CreateConnectedServiceRequest> ConnectedServices { get; init; }
    };
}