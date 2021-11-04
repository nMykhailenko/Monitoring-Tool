using Microsoft.Extensions.DependencyInjection;

namespace MonitoringTool.Common.Interfaces.Configuration
{
    /// <summary>
    /// Inject module.
    /// </summary>
    public interface IInjectModule
    {
        /// <summary>
        /// Load dependencies.
        /// </summary>
        /// <param name="services">Current service collection.</param>
        /// <returns>Updated service collection.</returns>
        IServiceCollection Load(IServiceCollection services);
    }
}