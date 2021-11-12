// using FluentAssertions;
// using Microsoft.Extensions.DependencyInjection;
// using MonitoringTool.Application.Interfaces.Services.Communication.Http;
// using MonitoringTool.Common.Extensions;
// using MonitoringTool.Infrastructure.Modules;
// using Xunit;
//
// namespace MonitoringTool.Infrastructure.UnitTests.Modules
// {
//     public class CommunicationModuleTests
//     {
//         [Fact]
//         public void CommunicationModule_ShouldLoad_AllNecessaryDI()
//         {
//             // arrange
//             IServiceCollection serviceCollection = new ServiceCollection();
//             serviceCollection.AddHttpClient();            
//             // act
//             serviceCollection = serviceCollection.RegisterModule<CommunicationModule>();
//
//             // assert
//             var serviceProvider = serviceCollection.BuildServiceProvider();
//             var httpCommunicationService = serviceProvider.GetService<IHttpCommunicationService>();
//
//             httpCommunicationService.Should().NotBeNull();
//         }
//     }
// }