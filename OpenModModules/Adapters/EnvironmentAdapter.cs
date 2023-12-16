using Hydriuk.UnturnedModules.Adapters;
using OpenMod.API.Plugins;
using System;
using System.Linq;
using System.Reflection;

namespace Hydriuk.OpenModModules.Adapters
{
    public class EnvironmentAdapter<TPlugin> : IEnvironmentAdapter
    {
        public string Directory { get => _plugin.WorkingDirectory; }

        private readonly IOpenModPlugin _plugin;

        public EnvironmentAdapter(IPluginActivator pluginActivator)
        {
            Assembly pluginAssembly = typeof(TPlugin).Assembly;

            IOpenModPlugin? plugin = pluginActivator.ActivatedPlugins
                .FirstOrDefault(plugin => plugin.GetType().Assembly == pluginAssembly);

            _plugin = plugin ??
                throw new Exception("Plugin not found. Make sure the plugin finished loading");
        }
    }
}