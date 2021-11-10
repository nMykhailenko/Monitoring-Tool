namespace MonitoringTool.Application.Models.ResponseModels.Errors
{
    public struct EntityIsAlreadyExists
    {
        public EntityIsAlreadyExists(string message)
        {
            Code = "EntityIsAlreadyExists";
            Message = message;
        }
        
        public string Code { get; init; }
        public string Message { get; init; }
    }
}