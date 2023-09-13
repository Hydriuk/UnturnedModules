using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.Events;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class ServiceAdapter : IServiceAdapter
    {
        private readonly IPluginActivator _pluginActivator;

        public ServiceAdapter(IPluginActivator pluginActivator)
        {
            _pluginActivator = pluginActivator;
        }

        public async Task<TService> GetServiceAsync<TService>()
        {
            Assembly pluginAssembly = typeof(TService).Assembly;

            // Try to get the plugin if already activated
            IOpenModPlugin? plugin = null;
            foreach (var activatedPlugin in _pluginActivator.ActivatedPlugins)
            {
                if(activatedPlugin.GetType().Assembly == pluginAssembly)
                {
                    plugin = activatedPlugin;
                    break;
                }
            }

            // Try to activate plugin
            if (plugin is null)
            {
                await Console.Out.WriteLineAsync("[Hydriuk.Plugins.Tools] - Plugin not activated. Activating plugin");
                plugin = await _pluginActivator.TryActivatePluginAsync(pluginAssembly);
            }

            if (plugin is null)
                throw new Exception("Plugin could not be activated");

            if (plugin is not IAdaptablePlugin adaptablePlugin)
                throw new Exception("Plugin does not implement IAdaptablePlugin");

            return adaptablePlugin.ServiceProvider.GetRequiredService<TService>();
        }
    }
}
