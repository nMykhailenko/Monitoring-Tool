using System.Collections.Generic;

namespace MonitoringTool.Application.Models.ResponseModels.ConnectedClient
{
    public record ConnectedClientResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        
        public ICollection<ConnectedServiceResponse> ConnectedServices { get; set; } = null!;
    };
}