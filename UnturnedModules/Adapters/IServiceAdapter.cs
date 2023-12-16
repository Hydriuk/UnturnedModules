using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IServiceAdapter : IDisposable
    {
        /// <summary>
        /// Gets a service from the calling plugin.
        /// Do not call if the plugin is not loaded, or this method will cause a dead lock.
        /// </summary>
        /// <typeparam name="TService">The service to resolve</typeparam>
        /// <returns>The resolved service</returns>
        Task<TService> GetServiceAsync<TService>() where TService : notnull;

        /// <summary>
        /// Gets a service from the plugin defined in the assembly.
        /// If the plugin is not loaded yet, this method will wait for the plugin to be loaded.
        /// </summary>
        /// <typeparam name="TService">The service to resolve</typeparam>
        /// <param name="pluginAssembly">Assembly containing the plugin</param>
        /// <returns>The resolved service</returns>
        Task<TService> GetServiceAsync<TService>(Assembly pluginAssembly) where TService : notnull;
        TService GetService<TService>() where TService : notnull;
    }
}