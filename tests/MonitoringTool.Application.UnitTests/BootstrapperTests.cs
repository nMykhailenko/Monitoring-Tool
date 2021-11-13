using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MonitoringTool.Application.UnitTests
{
    public class BootstrapperTests
    {
        [Fact]
        public void AddApplication_Should_Add_All_Needed_Services()
        {
            // arrange
            IServiceCollection serviceCollection = new ServiceCollection();

            // act
            var actualServiceCollection = serviceCollection.AddApplication();
            
            // assert
            var serviceProvider = actualServiceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();

            mapper.Should().NotBeNull();
        }
    }
}