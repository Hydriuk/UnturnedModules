using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Plugins;

namespace Hydriuk.RocketModModules.Adapters
{
    public class EnvironmentAdapter : IEnvironmentAdapter
    {
        private readonly RocketPlugin _plugin;

        public EnvironmentAdapter(RocketPlugin plugin)
        {
            _plugin = plugin;
        }

        public string Directory => _plugin.Directory;
    }
}