using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IServiceAdapter : IDisposable
    {
        /// <summary>
        /// Gets a service from the assembly the service is defined in.
        /// Do not await this method if the plugin is not loaded, or it will cause a dead lock.
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

        /// <summary>
        /// Gets a service from the current plugin.
        /// </summary>
        /// <typeparam name="TService">The service to resolve</typeparam>
        /// <returns>The resolved service</returns>
        /// <exception cref="Exception">The Plugin was not found or was not loaded yet</exception>
        TService GetService<TService>() where TService : notnull;
    }
}