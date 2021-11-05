namespace MonitoringTool.Application.Models.RequestModels.ConnectedClient
{
    public record CreateConnectedServiceRequest
    {
        public string Name { get; init; } = null!;
        public string BaseUrl { get; init; } = null!;
    };
}