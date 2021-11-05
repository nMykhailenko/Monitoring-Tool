using System.Linq;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MonitoringTool.Application.Mappers;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Domain.Entities;
using Xunit;

namespace MonitoringTool.Application.UnitTests.Mappers
{
    public class ConnectedClientMapTests
    {
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IMapper _sut;

        public ConnectedClientMapTests()
        {
            _mapperConfiguration = new MapperConfiguration(
                cfg => cfg.AddProfile<ConnectedClientMap>());
            _sut = _mapperConfiguration.CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            // assert
            _mapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_ShouldMap_CreateConnectedClientRequest_To_ConnectedClient_InProperWay()
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

            // act
            var actual = _sut.Map<ConnectedClient>(createConnectedClientRequest);
            
            // assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ConnectedClient>();
            actual.Name.Should().BeEquivalentTo(createConnectedClientRequest.Name);
            actual.IsActive.Should().Be(true);
            actual.ConnectedServices.Should().HaveCount(connectedServicesPerClientCount); 
            actual.ConnectedServices
                .Select(c => c.Name)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.Name));
            actual.ConnectedServices
                .Select(c => c.BaseUrl)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.BaseUrl));
            actual.ConnectedServices.Select(c => c.IsActive)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.IsActive));
        }
        
        [Fact]
        public void AutoMapper_ShouldMap_ConnectedClient_To_ConnectedClientResponse_InProperWay()
        {
            // arrange
            const int connectedServicesPerClientCount = 3;
            var connectedClient = new Faker<ConnectedClient>()
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .Generate(connectedServicesPerClientCount))
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .Generate();

            // act
            var actual = _sut.Map<ConnectedClient>(connectedClient);
            
            // assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ConnectedClient>();
            actual.Id.Should().Be(connectedClient.Id);
            actual.Name.Should().BeEquivalentTo(connectedClient.Name);
            actual.IsActive.Should().Be(connectedClient.IsActive);
            actual.ConnectedServices.Should().HaveCount(connectedServicesPerClientCount); 
            actual.ConnectedServices
                .Select(c => c.Name)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.Name));
            actual.ConnectedServices
                .Select(c => c.BaseUrl)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.BaseUrl));
            actual.ConnectedServices
                .Select(c => c.Id)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.Id));
            actual.ConnectedServices.Select(c => c.IsActive)
                .Should()
                .Equal(actual.ConnectedServices.Select(c => c.IsActive));
        }
    }
}