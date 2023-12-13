using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Events;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class ServiceAdapter : IServiceAdapter
    {
        private readonly IPluginActivator _pluginActivator;
        private readonly ILogger<OpenModModules> _logger;

        private TaskCompletionSource<object>? _loadedTask;

        public ServiceAdapter(IPluginActivator pluginActivator, ILogger<OpenModModules> logger)
        {
            _pluginActivator = pluginActivator;
            _logger = logger;

            PluginsLoadedListener.OpenModLoaded += OnOpenModLoaded;
        }

        public void Dispose()
        {
            PluginsLoadedListener.OpenModLoaded -= OnOpenModLoaded;
        }

        public async Task<TService> GetServiceAsync<TService>()
        {
            Assembly pluginAssembly = typeof(TService).Assembly;

            IAdaptablePlugin plugin = await GetPlugin(pluginAssembly);

            return plugin.ServiceProvider.GetRequiredService<TService>();
        }

        public async Task<TService> GetServiceAsync<TPluginAssembly, TService>() where TPluginAssembly : IAdaptablePlugin
        {
            Assembly pluginAssembly = typeof(TPluginAssembly).Assembly;

            IAdaptablePlugin plugin = await GetPlugin(pluginAssembly);

            return plugin.ServiceProvider.GetRequiredService<TService>();
        }

        public async Task<IAdaptablePlugin> GetPlugin(Assembly pluginAssembly)
        {
            // Try to get the plugin if already activated
            IOpenModPlugin? plugin = TryGetPlugin(pluginAssembly);

            // Wait for plugin to be activated through openmod
            if (plugin is null)
            {
                _loadedTask = new TaskCompletionSource<object>();

                _logger.LogInformation($"Loading external service. Plugin {pluginAssembly.FullName} not activated. Waiting for it to load");

                await _loadedTask.Task;

                plugin = TryGetPlugin(pluginAssembly);
            }

            if (plugin is null)
            {
                _logger.LogError("Plugin could not be activated");
                throw new NullReferenceException("plugin is null");
            }

            if (plugin is not IAdaptablePlugin adaptablePlugin)
            {
                _logger.LogError("Plugin does not implement IAdaptablePlugin");
                throw new Exception("plugin does not implement IAdaptablePlugin");
            }

            return adaptablePlugin;
        }

        private IOpenModPlugin? TryGetPlugin(Assembly pluginAssembly)
        {
            foreach (var activatedPlugin in _pluginActivator.ActivatedPlugins)
            {
                if (activatedPlugin.GetType().Assembly == pluginAssembly)
                {
                    return activatedPlugin;
                }
            }

            return null;
        }

        private void OnOpenModLoaded(object sender, object args)
        {
            _loadedTask?.SetResult(args);
        }

        private class PluginsLoadedListener : IEventListener<OpenModInitializedEvent>
        {
            public static EventHandler? OpenModLoaded;

            public Task HandleEventAsync(object? sender, OpenModInitializedEvent @event)
            {
                OpenModLoaded?.Invoke(sender, null);

                return Task.CompletedTask;
            }
        }
    }
}