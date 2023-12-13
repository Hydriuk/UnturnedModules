using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient)]
    internal class EnvironmentAdapter : IEnvironmentAdapter
    {
        private readonly IServiceAdapter _serviceAdapter;

        public EnvironmentAdapter(IServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public async Task<string> GetDirectory<T>() where T : IAdaptablePlugin
        {
            Assembly pluginAssembly = typeof(T).Assembly;

            IAdaptablePlugin plugin = await _serviceAdapter.GetPlugin(pluginAssembly);

            return plugin.WorkingDirectory;
        }
    }
}