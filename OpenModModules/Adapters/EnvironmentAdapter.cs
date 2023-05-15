using Hydriuk.UnturnedModules.API.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;

namespace Hydriuk.OpenModModules.Adapters
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient)]
    public class EnvironmentAdapter : IEnvironmentAdapter
    {
        private readonly IOpenModPlugin _plugin;

        public EnvironmentAdapter(IOpenModPlugin plugin)
        {
            _plugin = plugin;
        }

        public string Directory { get => _plugin.WorkingDirectory; }
    }
}