using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Reflection;

namespace Hydriuk.RocketModModules.Adapters
{
    public class EnvironmentAdapter : IEnvironmentAdapter
    {
        public string Directory => _plugin.Directory;

        private readonly RocketPlugin _plugin;

        public EnvironmentAdapter(IOpenModPlugin)
        {
            Assembly pluginAssembly = typeof(TPlugin).Assembly;

            _plugin = R.Plugins.GetPlugin(pluginAssembly) as RocketPlugin;
        }
    }
}