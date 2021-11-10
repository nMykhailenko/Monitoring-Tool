using FluentValidation;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;

namespace MonitoringTool.Application.Validators.ConnectedClient
{
    public class CreateConnectedClientRequestValidator : AbstractValidator<CreateConnectedClientRequest>
    {
        public CreateConnectedClientRequestValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.ConnectedServices).Must(x => x.Count > 0);
            RuleForEach(x => x.ConnectedServices)
                .SetValidator(new CreateConnectedServiceRequestValidator());
        }
    }
}