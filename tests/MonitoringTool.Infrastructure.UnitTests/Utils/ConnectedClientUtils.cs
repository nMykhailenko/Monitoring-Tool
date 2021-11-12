using System.Collections.Generic;
using Bogus;
using MonitoringTool.Application.Models.RequestModels.ConnectedClient;
using MonitoringTool.Domain.Entities;

namespace MonitoringTool.Infrastructure.UnitTests.Utils
{
    public static class ConnectedClientUtils
    {
        public static CreateConnectedClientRequest GetCreateConnectedClientRequest(int connectedServicesPerClientCount)
        {
            var createConnectedClientRequest = new Faker<CreateConnectedClientRequest>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<CreateConnectedServiceRequest>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .Generate(connectedServicesPerClientCount))
                .Generate();

            return createConnectedClientRequest;
        }

        public static ConnectedClient GetConnectedClient(int connectedServicesPerClientCount)
        {
            var connectedClient = new Faker<ConnectedClient>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .Generate(connectedServicesPerClientCount))
                .Generate();

            return connectedClient;
        }
        public static IEnumerable<ConnectedClient> GetConnectedClients(int connectedServicesPerClientCount)
        {
            var connectedClients = new Faker<ConnectedClient>()
                .RuleFor(cc => cc.Name, _ => _.Name.FullName())
                .RuleFor(cc => cc.IsActive, _ => true)
                .RuleFor(cc
                    => cc.ConnectedServices, _ => new Faker<ConnectedService>()
                    .RuleFor(cs => cs.BaseUrl, _ => _.Internet.UrlRootedPath())
                    .RuleFor(cs => cs.Name, _ => _.Name.FullName())
                    .Generate(connectedServicesPerClientCount))
                .Generate(connectedServicesPerClientCount);

            return connectedClients;
        }
    }
}