using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Common.Interfaces.Configuration;

namespace MonitoringTool.Common.Extensions
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register DI module.
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="services">Current service collection.</param>
        /// <returns>Updated service collection.</returns>
        public static IServiceCollection RegisterModule<TModule>(this IServiceCollection services)
            where TModule : IInjectModule, new()
        {
            var module = (IInjectModule)FormatterServices.GetUninitializedObject(typeof(TModule));
            return module.Load(services);
        }

        /// <summary>
        /// Register DI module.
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="services">Current service collection.</param>
        /// <param name="module">DI module.</param>
        /// <returns>Updated service collection.</returns>
        public static IServiceCollection RegisterModule<TModule>(this IServiceCollection services, TModule module)
            where TModule : IInjectModule => module.Load(services);
    }
}