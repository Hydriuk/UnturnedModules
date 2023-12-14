#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IServiceAdapter : IDisposable
    {
        Task<TService> GetServiceAsync<TPluginAssembly, TService>() where TPluginAssembly : IAdaptablePlugin;
        Task<IAdaptablePlugin> GetPluginAsync(Assembly pluginAssembly);
        IAdaptablePlugin GetAdaptablePlugin();
        TService GetService<TService>();
    }

#if OPENMOD
    [Service]
#endif
    internal interface IUnsafeServiceAdapter : IServiceAdapter
    {
        /// <summary>
        /// Gets the adaptable plugin instance from an assembly.
        /// This method is not safe to use on an unloaded plugin
        /// </summary>
        /// <param name="pluginAssembly">Assembly of the plugin</param>
        /// <returns>The plugin</returns>
        /// <exception cref="Exception">Plugin was not found</exception>
        /// <exception cref="Exception">Plugin does not implement IAdaptablePlugin</exception>
        IAdaptablePlugin GetAdaptablePlugin(Assembly pluginAssembly);

        /// <summary>
        /// Gets a service from a plugin
        /// This method is not safe to use on an unloaded plugin
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="pluginAssembly">Assembly of the plugin to get the service from</param>
        /// <returns>The service</returns>        
        /// <exception cref="Exception">Plugin was not found</exception>
        /// <exception cref="Exception">Plugin does not implement IAdaptablePlugin</exception>
        TService GetService<TService>(Assembly pluginAssembly);
    }
}