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
            RuleFor(x => x.ConnectedServices)
                .NotNull()
                .When(x => x.ConnectedServices.Count > 0)
                .ForEach(x => x.SetValidator(new CreateConnectedServiceRequestValidator()));
        }
    }
}