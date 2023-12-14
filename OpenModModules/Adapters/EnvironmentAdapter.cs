using Hydriuk.UnturnedModules.Adapters;
using OpenMod.API.Plugins;
using System;

namespace Hydriuk.OpenModModules.Adapters
{
    internal class EnvironmentAdapter<TPlugin> : IEnvironmentAdapter<TPlugin> where TPlugin : IAdaptablePlugin
    {
        public string Directory { get => _plugin.WorkingDirectory; }

        private readonly IOpenModPlugin _plugin;

        public EnvironmentAdapter(IPluginAccessor<TPlugin> plugin)
        {
            _plugin = plugin.Instance ??
                throw new Exception("Plugin not found. Make sure the plugin finished loading");
        }
    }
}