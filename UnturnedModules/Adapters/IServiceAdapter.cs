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
        Task<IAdaptablePlugin> GetPlugin(Assembly pluginAssembly);
    }
}