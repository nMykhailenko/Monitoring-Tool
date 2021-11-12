using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MonitoringTool.Application.Interfaces.Database.Repositories;
using MonitoringTool.Application.Interfaces.Services;
using MonitoringTool.Application.Interfaces.Services.Communication.Http;
using MonitoringTool.Application.Models.ResponseModels.HealthCheck;
using MonitoringTool.Domain.Entities;
using MonitoringTool.Infrastructure.Services;
using MonitoringTool.Infrastructure.UnitTests.Utils;
using Moq;
using Xunit;

namespace MonitoringTool.Infrastructure.UnitTests.Services
{
    public class HealthCheckServiceTests
    {
        private IHealthCheckService _sut = null!;
        private readonly Mock<IConnectedClientRepository> _connectedClientRepositoryMock = null!;
        private readonly Mock<IHttpCommunicationService> _httpCommunicationServiceMock = null!;

        public HealthCheckServiceTests()
        {
            _connectedClientRepositoryMock = new Mock<IConnectedClientRepository>();
            _httpCommunicationServiceMock = new Mock<IHttpCommunicationService>();
        }
        
        [Fact]
        public async Task CheckAsync_ShouldRun_WithoutExceptions()
        {
            // arrange
            const int count = 2;
            var healthCheckResponse = new Faker<HealthCheckResponse>().Generate();
            var connectedClients = ConnectedClientUtils.GetConnectedClients(count);

            _connectedClientRepositoryMock
                .Setup(x => x.GetActiveAsync(CancellationToken.None))
                .ReturnsAsync(connectedClients);

            _httpCommunicationServiceMock
                .Setup(x
                    => x.SendAsync<HealthCheckResponse>(
                        It.IsAny<string>(), 
                        HttpMethod.Get, 
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(healthCheckResponse);
            
            _sut = new HealthCheckService(
                _connectedClientRepositoryMock.Object,
                _httpCommunicationServiceMock.Object);
            
            // act
            await _sut.CheckAsync(CancellationToken.None);
            
            // assert
            _connectedClientRepositoryMock
                .Verify(x => x.GetActiveAsync(CancellationToken.None), Times.Exactly(1));
            _httpCommunicationServiceMock
                .Verify(
                    x => x.SendAsync<HealthCheckResponse>(
                        It.IsAny<string>(), 
                        HttpMethod.Get, 
                        It.IsAny<CancellationToken>()),
                    Times.Exactly(count * count));
        }
    }
}