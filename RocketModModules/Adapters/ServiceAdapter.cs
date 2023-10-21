using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ServiceAdapter : IServiceAdapter
    {
        private TaskCompletionSource<object>? _loadedTask;

        public ServiceAdapter(IAdaptablePlugin plugin)
        {
            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;
        }

        public void Dispose()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;
        }

        public async Task<TService> GetServiceAsync<TService>()
        {
            Assembly pluginAssembly = typeof(TService).Assembly;

            // Try to get the plugin if already activated
            IRocketPlugin plugin = R.Plugins.GetPlugin(pluginAssembly);

            // Wait for plugin to be activated through openmod
            if (plugin is null)
            {
                _loadedTask = new TaskCompletionSource<object>();

                await Console.Out.WriteLineAsync("[Hydriuk.Plugins.Tools] - Loading {typeof(TService)}. Plugin {pluginAssembly.FullName} not activated. Waiting for plugin to load");
                await _loadedTask.Task;

                plugin = R.Plugins.GetPlugin(pluginAssembly);
            }

            if (plugin is null)
                throw new Exception("[Hydriuk.Plugins.Tools] - Plugin could not be activated");

            if (plugin is not IAdaptablePlugin adaptablePlugin)
                throw new Exception("[Hydriuk.Plugins.Tools] - Plugin does not implement IAdaptablePlugin");

            PropertyInfo serviceInfo = plugin
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(property => property.PropertyType == typeof(TService));

            return (TService)serviceInfo.GetValue(plugin);
        }

        private void OnPluginsLoaded()
        {
            _loadedTask?.SetResult(null);
        }
    }
}