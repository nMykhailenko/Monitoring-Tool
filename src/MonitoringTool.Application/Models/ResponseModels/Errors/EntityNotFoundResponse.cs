namespace MonitoringTool.Application.Models.ResponseModels.Errors
{
    public struct EntityNotFoundResponse
    {
        public EntityNotFoundResponse(string message)
        {
            Code = "EntityNotFound";
            Message = message;
        }
        
        public string Code { get; init; }
        public string Message { get; init; }
    }
}