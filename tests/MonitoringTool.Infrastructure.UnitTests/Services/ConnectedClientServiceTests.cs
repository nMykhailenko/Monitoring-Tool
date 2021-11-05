using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Domain.Entities;
using MonitoringTool.Infrastructure.Services;
using Moq;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Services
{
    public class ConnectedClientServiceTests
    {
        private IConnectedClientService _sut;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConnectedClientRepository> _connectedClientRepositoryMock;

        public ConnectedClientServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _connectedClientRepositoryMock = new Mock<IConnectedClientRepository>();
        }

        [Fact]
        public async Task AddConnectedClient_ShouldReturn_AddedConnectedClient()
        {
            // arrange
            const int connectedServicesPerClientCount = 3;
            var createConnectedClientRequest = new Faker<CreateConnectedClientRequest>()
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<CreateConnectedServiceRequest>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .Generate(connectedServicesPerClientCount))
                .Generate();

            _connectedClientRepositoryMock
                .Setup(x
                    => x.AddAsync(It.IsAny<ConnectedClient>(), It.IsAny<CancellationToken>()));

            _sut = new ConnectedClientService(_mapperMock.Object, _connectedClientRepositoryMock.Object);
            
            // act
            await _sut.AddAsync(createConnectedClientRequest, CancellationToken.None);

            // assert
            _connectedClientRepositoryMock
                .Verify(x 
                    => x.AddAsync(
                        It.IsAny<ConnectedClient>(), 
                        It.IsAny<CancellationToken>()), 
                    Times.Exactly(1));
            _mapperMock.Verify(x 
                => x.Map<ConnectedClient>(createConnectedClientRequest), Times.Exactly(1));
        }
    }
}