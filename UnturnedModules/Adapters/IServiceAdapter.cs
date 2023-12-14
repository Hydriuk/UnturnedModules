using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IServiceAdapter : IDisposable
    {
        Task<TService> GetServiceAsync<TPluginAssembly, TService>() where TPluginAssembly : IAdaptablePlugin;
        Task<IAdaptablePlugin> GetPluginAsync(Assembly pluginAssembly);
        IAdaptablePlugin GetAdaptablePlugin();
        TService GetService<TService>();
    }
}