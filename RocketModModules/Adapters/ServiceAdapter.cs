using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ServiceAdapter : IServiceAdapter
    {
        private TaskCompletionSource<object>? _loadedTask;

        public ServiceAdapter()
        {
            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;
        }

        public void Dispose()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;
        }

        public async Task<TService> GetServiceAsync<TPluginAssembly, TService>() where TPluginAssembly : IAdaptablePlugin
        {
            Assembly pluginAssembly = typeof(TService).Assembly;

            IAdaptablePlugin plugin = await GetPluginAsync(pluginAssembly);

            PropertyInfo serviceInfo = plugin
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(property => property.PropertyType == typeof(TService));

            return (TService)serviceInfo.GetValue(plugin);
        }

        public TService GetService<TService>() => GetService<TService>(Assembly.GetCallingAssembly());
        private TService GetService<TService>(Assembly pluginAssembly)
        {
            IRocketPlugin plugin = R.Plugins.GetPlugin(pluginAssembly) ??
                throw new Exception($"Plugin {pluginAssembly.FullName} not found");

            PropertyInfo serviceInfo = plugin
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(property => property.PropertyType == typeof(TService));

            return (TService)serviceInfo.GetValue(plugin);
        }

        public async Task<IAdaptablePlugin> GetPluginAsync(Assembly pluginAssembly)
        {
            // Try to get the plugin if already activated
            IRocketPlugin plugin = R.Plugins.GetPlugin(pluginAssembly);

            // Wait for plugin to be activated through openmod
            if (plugin is null)
            {
                _loadedTask = new TaskCompletionSource<object>();

                Logger.Log($"Loading external service. Plugin {pluginAssembly.FullName} not activated. Waiting for it to load");

                await _loadedTask.Task;

                Logger.Log($"Plugin loaded");

                plugin = R.Plugins.GetPlugin(pluginAssembly);
            }

            if (plugin is null)
            {
                Logger.LogError("Plugin could not be activated");
                throw new NullReferenceException("Variable plugin is null");
            }

            if (plugin is not IAdaptablePlugin adaptablePlugin)
            {
                Logger.LogError("Plugin does not implement IAdaptablePlugin");
                throw new Exception("Variable plugin does not implement IAdaptablePlugin");
            }

            return adaptablePlugin;
        }

        public IAdaptablePlugin GetAdaptablePlugin() => GetAdaptablePlugin(Assembly.GetCallingAssembly());
        private IAdaptablePlugin GetAdaptablePlugin(Assembly pluginAssembly)
        {
            IRocketPlugin plugin = R.Plugins.GetPlugin(pluginAssembly) ??
                throw new Exception($"Plugin {pluginAssembly.FullName} not found");

            return plugin as IAdaptablePlugin ??
                throw new Exception($"Plugin {pluginAssembly.FullName} is not a IAdaptablePlugin");
        }

        private void OnPluginsLoaded()
        {
            _loadedTask?.SetResult(null);
        }
    }
}