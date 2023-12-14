using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Plugins;
using System.Reflection;

namespace Hydriuk.RocketModModules.Adapters
{
    internal class EnvironmentAdapter : IEnvironmentAdapter
    {
        private readonly IUnsafeServiceAdapter _serviceAdapter;

        public EnvironmentAdapter(IUnsafeServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public string GetDirectory()
        {
            RocketPlugin plugin = _serviceAdapter.GetAdaptablePlugin(Assembly.GetCallingAssembly()) as RocketPlugin;

            return plugin.Directory;
        }
    }
}