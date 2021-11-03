namespace MonitoringTool.Domain.Entities
{
    public class ConnectedService
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}