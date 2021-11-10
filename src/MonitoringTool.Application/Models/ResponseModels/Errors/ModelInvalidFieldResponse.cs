namespace MonitoringTool.Application.Models.ResponseModels.Errors
{
    public record ModelInvalidFieldResponse
    {
        public ModelInvalidFieldResponse(string fieldName, string message)
        {
            FieldName = fieldName;
            Message = message;
        }

        public string FieldName { get; init; }
        public string Message { get; init; }   
    }
}