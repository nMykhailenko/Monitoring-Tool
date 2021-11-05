namespace MonitoringTool.Application.Models.ResponseModels.ConnectedClient
{
    public record ConnectedServiceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public bool IsActive { get; set; }
    };
}