using Bogus;
using FluentAssertions;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Validators.ConnectedClient;
using Xunit;

namespace MonitoringTool.Application.UnitTests.Validators
{
    public class ConnectedClientValidatorTests
    {
        [Fact]
        public void CreateConnectedClientRequestValidator_Should_Return_True_If_Model_Is_Valid()
        {
            // arrange 
            const int connectedServicesPerClientCount = 3;
            var createConnectedClientRequest = new Faker<CreateConnectedClientRequest>()
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<CreateConnectedServiceRequest>()
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .Generate(connectedServicesPerClientCount))
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .Generate();
            var validator = new CreateConnectedClientRequestValidator();
            
            // act
            var result = validator.Validate(createConnectedClientRequest);
            
            // assert
            result.IsValid.Should().Be(true);
        }
        
        [Fact]
        public void CreateConnectedClientRequestValidator_Should_Return_False_If_Model_Is_Not_Valid()
        {
            // arrange 
            var createConnectedClientRequest = new Faker<CreateConnectedClientRequest>().Generate();
            var validator = new CreateConnectedClientRequestValidator();
            
            // act
            var result = validator.Validate(createConnectedClientRequest);
            
            // assert
            result.IsValid.Should().Be(false);
        }
    }
}