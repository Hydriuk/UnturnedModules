using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class EnvironmentAdapter : IEnvironmentAdapter
    {
        private readonly IUnsafeServiceAdapter _serviceAdapter;

        public EnvironmentAdapter(IUnsafeServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public string GetDirectory()
        {
            Assembly pluginAssembly = Assembly.GetCallingAssembly();

            IAdaptablePlugin plugin = _serviceAdapter.GetAdaptablePlugin(pluginAssembly);

            return plugin.WorkingDirectory;
        }
    }
}