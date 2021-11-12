using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Mappers;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.ConnectedClient;
using MonitoringTool.Application.Models.ResponseModels.Errors;
using MonitoringTool.Domain.Entities;
using MonitoringTool.Infrastructure.Services;
using Moq;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Services
{
    public class ConnectedClientServiceTests
    {
        private IConnectedClientService _sut;
        private readonly IMapper _mapper;
        private readonly Mock<IConnectedClientRepository> _connectedClientRepositoryMock;

        public ConnectedClientServiceTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                cfg => cfg.AddProfile<ConnectedClientMap>());
            _mapper = mapperConfiguration.CreateMapper();

            _connectedClientRepositoryMock = new Mock<IConnectedClientRepository>();
        }

        [Fact]
        public async Task GetByName_ShouldReturn_ConnectedClientResponse_If_ConnectedClient_With_Defined_Name_Is_Exists()
        {
            // arrange 
            const int connectedServicesPerClientCount = 3;
            const string connectedClientName = "test name";
            
            var expectedConnectedClient = new Faker<ConnectedClient>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .Generate(connectedServicesPerClientCount))
                .Generate();;
            
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedConnectedClient);
            
            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);

            // act 
            var actual = await _sut.GetByNameAsync(connectedClientName, CancellationToken.None);
            
            // assert
            var actualModel = (ConnectedClientResponse)actual.Value;
            actualModel.Id.Should().Be(expectedConnectedClient.Id);
            actualModel.Name.Should().BeEquivalentTo(expectedConnectedClient.Name);
            
            actualModel.ConnectedServices.Should().HaveCount(connectedServicesPerClientCount); 
            actualModel.ConnectedServices
                .Select(c => c.Name)
                .Should()
                .Equal(expectedConnectedClient.ConnectedServices.Select(c => c.Name));
            actualModel.ConnectedServices
                .Select(c => c.BaseUrl)
                .Should()
                .Equal(expectedConnectedClient.ConnectedServices.Select(c => c.BaseUrl));
            actualModel.ConnectedServices.Select(c => c.IsActive)
                .Should()
                .Equal(expectedConnectedClient.ConnectedServices.Select(c => c.IsActive));
        }
        
        [Fact]
        public async Task GetByName_ShouldReturn_EntityNotFoundResponse_If_ConnectedClient_With_Defined_Name_Not_Exists()
        {
            // arrange 
            const string expectedCode = "EntityNotFound";
            const string connectedClientName = "test name";
            var expectedErrorMessage = $"Connected client with name: {connectedClientName} not found.";
            
            ConnectedClient? currentConnectedClient = null;
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentConnectedClient);
            
            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);

            // act 
            var actual = await _sut.GetByNameAsync(connectedClientName, CancellationToken.None);
            
            // assert
            actual.Value.Should().BeOfType(typeof(EntityNotFoundResponse));
            
            var actualModel = (EntityNotFoundResponse)actual.Value;
            actualModel.Message.Should().BeEquivalentTo(expectedErrorMessage);
            actualModel.Code.Should().BeEquivalentTo(expectedCode);
        }

        [Fact]
        public async Task AddConnectedClient_ShouldReturn_AddedConnectedClient()
        {
            // arrange
            const int connectedServicesPerClientCount = 3;
            var createConnectedClientRequest = new Faker<CreateConnectedClientRequest>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<CreateConnectedServiceRequest>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .Generate(connectedServicesPerClientCount))
                .Generate();

            var expectedConnectedClient = _mapper.Map<ConnectedClient>(createConnectedClientRequest);
            expectedConnectedClient.Id = 10;

            ConnectedClient? currentConnectedClient = null;
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(createConnectedClientRequest.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentConnectedClient);
            
            _connectedClientRepositoryMock
                .Setup(x
                    => x.AddAsync(It.IsAny<ConnectedClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedConnectedClient);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);
            
            // act
           var actual = await _sut.AddAsync(createConnectedClientRequest, CancellationToken.None);

            // assert
            _connectedClientRepositoryMock
                .Verify(x 
                    => x.GetByNameAsync(createConnectedClientRequest.Name, CancellationToken.None), Times.Exactly(1));
            _connectedClientRepositoryMock
                .Verify(x 
                    => x.AddAsync(
                        It.IsAny<ConnectedClient>(), 
                        It.IsAny<CancellationToken>()), 
                    Times.Exactly(1));
            actual.Value.Should().BeOfType(typeof(ConnectedClientResponse));
            
            var actualModel = (ConnectedClientResponse)actual.Value;
            actualModel.Id.Should().Be(expectedConnectedClient.Id);
            actualModel.Name.Should().BeEquivalentTo(expectedConnectedClient.Name);
            
            actualModel.ConnectedServices.Should().HaveCount(connectedServicesPerClientCount); 
            actualModel.ConnectedServices
                .Select(c => c.Name)
                .Should()
                .Equal(expectedConnectedClient.ConnectedServices.Select(c => c.Name));
            actualModel.ConnectedServices
                .Select(c => c.BaseUrl)
                .Should()
                .Equal(expectedConnectedClient.ConnectedServices.Select(c => c.BaseUrl));
            actualModel.ConnectedServices.Select(c => c.IsActive)
                .Should()
                .Equal(expectedConnectedClient.ConnectedServices.Select(c => c.IsActive));
        }

        [Fact]
        public async Task AddConnectedClient_ShouldReturn_EntityAlreadyExistResponse()
        {
            // arrange
            const string expectedCode = "EntityIsAlreadyExists";
            const int connectedServicesPerClientCount = 3;
            var createConnectedClientRequest = new Faker<CreateConnectedClientRequest>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<CreateConnectedServiceRequest>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .Generate(connectedServicesPerClientCount))
                .Generate();


            var expectedErrorMessage = $"The Connected Client with name: {createConnectedClientRequest.Name} is already exists." +
                                       $"If you want to add Connected Service for this client you should use another endpoint.";
            var currentConnectedClient = new Faker<ConnectedClient>().Generate();
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(createConnectedClientRequest.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentConnectedClient);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);

            // act
            var actual = await _sut.AddAsync(createConnectedClientRequest, CancellationToken.None);

            // assert
            _connectedClientRepositoryMock
                .Verify(x
                    => x.GetByNameAsync(createConnectedClientRequest.Name, CancellationToken.None), Times.Exactly(1));
            actual.Value.Should().NotBeNull().And.BeOfType(typeof(EntityIsAlreadyExists));
            
            var actualModel = (EntityIsAlreadyExists)actual.Value;
            actualModel.Message.Should().BeEquivalentTo(expectedErrorMessage);
            actualModel.Code.Should().BeEquivalentTo(expectedCode);
        }
    }
}