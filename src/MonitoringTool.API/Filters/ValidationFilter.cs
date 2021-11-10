using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MonitoringTool.Application.Models.ResponseModels.Errors;

namespace MonitoringTool.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage))
                    .ToArray();

                var fieldErrors = new List<ModelInvalidFieldResponse>();
                foreach (var error in errorsInModelState)
                {
                    var nestedErrors = error.Value
                        .Select(subError => new ModelInvalidFieldResponse(error.Key, subError));
                    fieldErrors.AddRange(nestedErrors);
                }
                
                var errorResponse = new ModelInvalidResponse(fieldErrors);

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }

            await next();
        }
    }
}