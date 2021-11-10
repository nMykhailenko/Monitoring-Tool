using System;
using FluentValidation;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;

namespace MonitoringTool.Application.Validators.ConnectedClient
{
    public class CreateConnectedServiceRequestValidator : AbstractValidator<CreateConnectedServiceRequest>
    {
        public CreateConnectedServiceRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.BaseUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.BaseUrl));
        }
    }
}