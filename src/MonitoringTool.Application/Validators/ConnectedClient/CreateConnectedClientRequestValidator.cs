using FluentValidation;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;

namespace MonitoringTool.Application.Validators.ConnectedClient
{
    public class CreateConnectedClientRequestValidator : AbstractValidator<CreateConnectedClientRequest>
    {
        public CreateConnectedClientRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleForEach(x => x.ConnectedServices)
                .SetValidator(new CreateConnectedServiceRequestValidator());
        }
    }
}