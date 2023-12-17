using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ServiceAdapter : IServiceAdapter
    {
        private TaskCompletionSource<object>? _loadedTask;
        private readonly Dictionary<Type, Type> _implementingType = new Dictionary<Type, Type>();

        public ServiceAdapter(RocketPlugin plugin)
        {
            var types = plugin.GetType()
                .Assembly.GetTypes()
                .Concat(Assembly.GetExecutingAssembly().GetTypes());

            var classes = types.Where(t => t.IsClass);

            _implementingType = types.Where(t => t.IsInterface)
                .ToDictionary(t => t, t => classes.FirstOrDefault(c => t.IsAssignableFrom(c)));

            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;
        }

        public void Dispose()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;
        }

        private void OnPluginsLoaded()
        {
            _loadedTask?.SetResult(null);
        }

        public Task<TService> GetServiceAsync<TService>() 
            where TService : notnull
        {
            return GetServiceAsync<TService>(typeof(TService).Assembly);
        }

        public async Task<TService> GetServiceAsync<TService>(Assembly pluginAssembly) 
            where TService : notnull
        {
            IRocketPlugin plugin = await GetPluginAsync(pluginAssembly);

            return GetService<TService>(plugin);
        }

        public TService GetService<TService>() 
            where TService : notnull
        {
            Assembly pluginAssembly = Assembly.GetCallingAssembly();

            IRocketPlugin plugin = R.Plugins.GetPlugin(pluginAssembly) ??
                throw new Exception($"Plugin {pluginAssembly.FullName} not found");

            return GetService<TService>(plugin);
        }

        private TService GetService<TService>(IRocketPlugin plugin) 
            where TService : notnull
        {
            PropertyInfo serviceInfo = plugin
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(property => property.PropertyType == _implementingType[typeof(TService)]);

            return (TService)serviceInfo?.GetValue(plugin) ??
                throw new Exception("Service could not be found");
        }

        private async Task<IRocketPlugin> GetPluginAsync(Assembly pluginAssembly)
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

            return plugin;
        }
    }
}