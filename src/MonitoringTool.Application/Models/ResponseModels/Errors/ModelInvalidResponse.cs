using System.Collections.Generic;

namespace MonitoringTool.Application.Models.ResponseModels.Errors
{
    public record ModelInvalidResponse
    {
        public ModelInvalidResponse(ICollection<ModelInvalidFieldResponse> errors)
        {
            Errors = errors;
            Code = "ModelIsNotValid";
        }

        public string Code { get; init; }
        public ICollection<ModelInvalidFieldResponse> Errors { get; init; }
    };
}