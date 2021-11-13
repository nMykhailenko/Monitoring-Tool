using System.Collections.Generic;
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
using MonitoringTool.Infrastructure.UnitTests.Utils;
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
        public async Task GetActiveAsync_ShouldReturn_Only_ConnectedClients_With_Active_Status()
        {
            // arrange
            const int connectedServicesPerClientCount = 3;
            var expectedConnectedClients = ConnectedClientUtils
                .GetConnectedClients(connectedServicesPerClientCount)
                .ToList();

            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedConnectedClients);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);

            // act
            var actual = (await _sut.GetActiveAsync(CancellationToken.None)).ToList();

            // assert
            actual.Count().Should().Be(expectedConnectedClients.Count);
            actual
                .Select(x => x.IsActive)
                .Should()
                .Equal(expectedConnectedClients.Select(c => c.IsActive));
        }

        [Fact]
        public async Task
            GetByName_ShouldReturn_ConnectedClientResponse_If_ConnectedClient_With_Defined_Name_Is_Exists()
        {
            // arrange 
            const int connectedServicesPerClientCount = 3;
            const string connectedClientName = "test name";
            var expectedConnectedClient = ConnectedClientUtils.GetConnectedClient(connectedServicesPerClientCount);

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
        public async Task
            GetByName_ShouldReturn_EntityNotFoundResponse_If_ConnectedClient_With_Defined_Name_Not_Exists()
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
        public async Task
            AddConnectedClient_ShouldReturn_AddedConnectedClient_If_ConnectedClient_With_Defined_Name_Not_Exists()
        {
            // arrange
            const int connectedServicesPerClientCount = 3;
            var createConnectedClientRequest =
                ConnectedClientUtils.GetCreateConnectedClientRequest(connectedServicesPerClientCount);

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
        public async Task
            AddConnectedClient_ShouldReturn_EntityAlreadyExistResponse_If_ConnectedClient_With_Defined_Name_Already_Exists()
        {
            // arrange
            const string expectedCode = "EntityIsAlreadyExists";
            const int connectedServicesPerClientCount = 3;
            var createConnectedClientRequest =
                ConnectedClientUtils.GetCreateConnectedClientRequest(connectedServicesPerClientCount);

            var expectedErrorMessage =
                $"The Connected Client with name: {createConnectedClientRequest.Name} is already exists." +
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

        [Fact]
        public async Task
            AddConnectedServicesToClientAsync_ShouldReturn_EntityNotFoundResponse_If_ConnectedClient_With_Defined_Name_Not_Exists()
        {
            // arrange 
            const string expectedCode = "EntityNotFound";
            const string connectedClientName = "test name";
            var expectedErrorMessage = $"Connected client with name: {connectedClientName} not found.";

            var createConnectedServiceRequests = new List<CreateConnectedServiceRequest>();
            ConnectedClient? currentConnectedClient = null;
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentConnectedClient);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);

            // act 
            var actual = await _sut.AddConnectedServicesToClientAsync(
                connectedClientName,
                createConnectedServiceRequests,
                CancellationToken.None);

            // assert
            actual.Value.Should().BeOfType(typeof(EntityNotFoundResponse));

            var actualModel = (EntityNotFoundResponse)actual.Value;
            actualModel.Message.Should().BeEquivalentTo(expectedErrorMessage);
            actualModel.Code.Should().BeEquivalentTo(expectedCode);
        }

        [Fact]
        public async Task
            AddConnectedServicesToClientAsync_ShouldReturn_ConnectedServiceResponses_If_ConnectedClient_With_Defined_Name_Exists()
        {
            // arrange 
            const int connectedServicesPerClientCount = 5;
            const int connectedServiceId = 5;
            const int connectedClientId = 2;
            const string connectedClientName = "test name";

            var expectedConnectedClient = new Faker<ConnectedClient>()
                .RuleFor(cc => cc.Id, _ => connectedClientId)
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.ConnectedClientId, _ => connectedClientId)
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .RuleFor(cs => cs.IsActive, _ => true)
                    .Generate(connectedServicesPerClientCount))
                .Generate();
            var notActiveConnectedService = new Faker<ConnectedService>()
                .RuleFor(cs => cs.Id, _ => connectedServiceId)
                .RuleFor(cs => cs.ConnectedClientId, _ => connectedClientId)
                .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                .RuleFor(cs => cs.IsActive, _ => false)
                .Generate();
            expectedConnectedClient.ConnectedServices.Add(notActiveConnectedService);

            var createConnectedServiceRequests = new List<CreateConnectedServiceRequest>();
            var createConnectedServiceRequestIsNotActive =
                expectedConnectedClient.ConnectedServices
                    .Where(x => !x.IsActive)
                    .Select(x => new CreateConnectedServiceRequest
                    {
                        Name = x.Name,
                        BaseUrl = x.BaseUrl
                    }).First();
            var createConnectedServiceRequestIsActive =
                expectedConnectedClient.ConnectedServices
                    .Where(x => x.IsActive)
                    .Select(x => new CreateConnectedServiceRequest
                    {
                        Name = x.Name,
                        BaseUrl = x.BaseUrl
                    }).First();
            createConnectedServiceRequests.Add(createConnectedServiceRequestIsActive);
            createConnectedServiceRequests.Add(createConnectedServiceRequestIsNotActive);

            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedConnectedClient);

            var addedConnectedService = new ConnectedService
            {
                Name = notActiveConnectedService.Name,
                BaseUrl = notActiveConnectedService.BaseUrl,
                IsActive = true,
                ConnectedClientId = connectedClientId,
                Id = connectedServiceId
            };
            _connectedClientRepositoryMock
                .Setup(x
                    => x.AddConnectedServiceAsync(It.IsAny<ConnectedService>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedConnectedService);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);


            // act 
            var actualResponse = await _sut.AddConnectedServicesToClientAsync(
                connectedClientName,
                createConnectedServiceRequests,
                CancellationToken.None);

            actualResponse.Value.Should().BeOfType(typeof(List<ConnectedServiceResponse>));

            var actualModel = (List<ConnectedServiceResponse>)actualResponse.Value;
            actualModel.Should().HaveCount(1);

            var actualConnectedClientResponse = actualModel.First();
            actualConnectedClientResponse.Id.Should().Be(connectedServiceId);
            actualConnectedClientResponse.IsActive.Should().Be(true);
            actualConnectedClientResponse.Name.Should().BeEquivalentTo(createConnectedServiceRequestIsNotActive.Name);
            actualConnectedClientResponse.BaseUrl.Should()
                .BeEquivalentTo(createConnectedServiceRequestIsNotActive.BaseUrl);
        }

        [Fact]
        public async Task
            AddConnectedServiceToClientAsync_ShouldReturn_EntityNotFoundResponse_If_ConnectedClient_With_Defined_Name_Not_Exists()
        {
            // arrange 
            const string expectedCode = "EntityNotFound";
            const string connectedClientName = "test name";
            var expectedErrorMessage = $"Connected client with name: {connectedClientName} not found.";

            var createConnectedServiceRequest = new CreateConnectedServiceRequest();
            ConnectedClient? currentConnectedClient = null;
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentConnectedClient);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);

            // act 
            var actual = await _sut.AddConnectedServiceToClientAsync(
                connectedClientName,
                createConnectedServiceRequest,
                CancellationToken.None);

            // assert
            actual.Value.Should().BeOfType(typeof(EntityNotFoundResponse));

            var actualModel = (EntityNotFoundResponse)actual.Value;
            actualModel.Message.Should().BeEquivalentTo(expectedErrorMessage);
            actualModel.Code.Should().BeEquivalentTo(expectedCode);
        }
        
        [Fact]
        public async Task
            AddConnectedServiceToClientAsync_ShouldReturn_ConnectedServiceResponse_If_ConnectedClient_With_Defined_Name_Exists()
        {
            // arrange 
            const int connectedServicesPerClientCount = 5;
            const int connectedServiceId = 5;
            const int connectedClientId = 2;
            const string connectedClientName = "test name";

            var expectedConnectedClient = new Faker<ConnectedClient>()
                .RuleFor(cc => cc.Id, _ => connectedClientId)
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.ConnectedClientId, _ => connectedClientId)
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .RuleFor(cs => cs.IsActive, _ => false)
                    .Generate(connectedServicesPerClientCount))
                .Generate();
            var createConnectedServiceRequestIsNotActive =
                expectedConnectedClient.ConnectedServices
                    .Where(x => !x.IsActive)
                    .Select(x => new CreateConnectedServiceRequest
                    {
                        Name = x.Name,
                        BaseUrl = x.BaseUrl
                    }).First();

            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedConnectedClient);

            var addedConnectedService = new ConnectedService
            {
                Name = createConnectedServiceRequestIsNotActive.Name,
                BaseUrl = createConnectedServiceRequestIsNotActive.BaseUrl,
                IsActive = true,
                ConnectedClientId = connectedClientId,
                Id = connectedServiceId
            };
            _connectedClientRepositoryMock
                .Setup(x
                    => x.AddConnectedServiceAsync(It.IsAny<ConnectedService>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedConnectedService);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);


            // act 
            var actualResponse = await _sut.AddConnectedServiceToClientAsync(
                connectedClientName,
                createConnectedServiceRequestIsNotActive,
                CancellationToken.None);

            actualResponse.Value.Should().BeOfType(typeof(ConnectedServiceResponse));

            var actualModel = (ConnectedServiceResponse)actualResponse.Value;
            actualModel.Id.Should().Be(connectedServiceId);
            actualModel.IsActive.Should().Be(true);
            actualModel.Name.Should().BeEquivalentTo(createConnectedServiceRequestIsNotActive.Name);
            actualModel.BaseUrl.Should()
                .BeEquivalentTo(createConnectedServiceRequestIsNotActive.BaseUrl);
        }
        
        [Fact]
        public async Task
            AddConnectedServiceToClientAsync_ShouldReturn_EntityIsAlreadyExists_If_ConnectedClient_With_Defined_Name_Has_ConnectedService_With_Defined_Name()
        {
            // arrange 
            const int connectedServicesPerClientCount = 5;
            const string expectedCode = "EntityIsAlreadyExists";
            const string connectedClientName = "test name";

            var expectedConnectedClient = new Faker<ConnectedClient>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .RuleFor(cs => cs.IsActive, _ => true)
                    .Generate(connectedServicesPerClientCount))
                .Generate();
            var createConnectedServiceRequestIsActive =
                expectedConnectedClient.ConnectedServices
                    .Where(x => x.IsActive)
                    .Select(x => new CreateConnectedServiceRequest
                    {
                        Name = x.Name,
                        BaseUrl = x.BaseUrl
                    }).First();

            var expectedErrorMessage = $"Connected service with name: {createConnectedServiceRequestIsActive.Name} is already " +
                                       $"assigned to Connected client with name: {connectedClientName}";
            
            _connectedClientRepositoryMock
                .Setup(x
                    => x.GetByNameAsync(connectedClientName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedConnectedClient);

            _sut = new ConnectedClientService(_mapper, _connectedClientRepositoryMock.Object);


            // act 
            var actualResponse = await _sut.AddConnectedServiceToClientAsync(
                connectedClientName,
                createConnectedServiceRequestIsActive,
                CancellationToken.None);

            actualResponse.Value.Should().BeOfType(typeof(EntityIsAlreadyExists));

            var actualModel = (EntityIsAlreadyExists)actualResponse.Value;
            actualModel.Message.Should().BeEquivalentTo(expectedErrorMessage);
            actualModel.Code.Should().BeEquivalentTo(expectedCode);
        }
    }
}