using System.Collections.Generic;

namespace MonitoringTool.Application.Models.RequestModels.ConnectedClient
{
    public record CreateConnectedClientRequest
    {
        public string Name { get; init; } = null!;
        public ICollection<CreateConnectedServiceRequest> ConnectedServices { get; init; } = null!;
    };
}