using Autofac;
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
    public class ServiceAdapter : IServiceAdapter
    {
        private readonly IPluginActivator _pluginActivator;
        private readonly ILogger _logger;

        private TaskCompletionSource<object>? _loadedTask;

        public ServiceAdapter(IPluginActivator pluginActivator, ILogger logger)
        {
            _pluginActivator = pluginActivator;
            _logger = logger;

            PluginsLoadedListener.OpenModLoaded += OnOpenModLoaded;
        }

        public void Dispose()
        {
            PluginsLoadedListener.OpenModLoaded -= OnOpenModLoaded;
        }

        public Task<TService> GetServiceAsync<TService>() where TService : notnull
        {
            return GetServiceAsync<TService>(typeof(TService).Assembly);
        }

        public async Task<TService> GetServiceAsync<TService>(Assembly pluginAssembly) where TService : notnull
        {
            IOpenModPlugin plugin = await GetPluginAsync(pluginAssembly);

            return GetService<TService>(plugin);
        }

        public TService GetService<TService>() where TService : notnull
        {
            return GetService<TService>(Assembly.GetCallingAssembly());
        }

        private TService GetService<TService>(Assembly pluginAssembly) 
            where TService : notnull
        {
            IOpenModPlugin plugin = TryGetPlugin(pluginAssembly) ??
                throw new Exception("Plugin not found");

            return GetService<TService>(plugin);
        }

        private TService GetService<TService>(IOpenModPlugin plugin) where TService : notnull
        {
            return plugin.LifetimeScope.Resolve<TService>();
        }

        private async Task<IOpenModPlugin> GetPluginAsync(Assembly pluginAssembly)
        {
            // Try to get the plugin if already activated
            IOpenModPlugin? plugin = TryGetPlugin(pluginAssembly);

            // Wait for plugin to be activated through openmod
            if (plugin is null)
            {
                _loadedTask = new TaskCompletionSource<object>();

                _logger.LogInformation($"Loading external service. Plugin {pluginAssembly.FullName} not activated. Waiting for it to load");

                await _loadedTask.Task;

                _logger.LogInformation($"Plugin loaded");

                plugin = TryGetPlugin(pluginAssembly);
            }

            if (plugin is null)
            {
                _logger.LogError("Plugin could not be activated");
                throw new NullReferenceException("Variable plugin is null");
            }

            return plugin;
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