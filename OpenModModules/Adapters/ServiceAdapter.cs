using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
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
    internal class ServiceAdapter : IServiceAdapter, IDisposable
    {
        private readonly IPluginActivator _pluginActivator;

        private TaskCompletionSource<object>? _loadedTask;

        public ServiceAdapter(IPluginActivator pluginActivator)
        {
            _pluginActivator = pluginActivator;

            PluginsLoadedListener.OpenModLoaded += OnOpenModLoaded;
        }

        public void Dispose()
        {
            PluginsLoadedListener.OpenModLoaded -= OnOpenModLoaded;
        }

        public async Task<TService> GetServiceAsync<TService>()
        {
            Assembly pluginAssembly = typeof(TService).Assembly;

            // Try to get the plugin if already activated
            IOpenModPlugin? plugin = TryGetPlugin(pluginAssembly);

            // Wait for plugin to be activated through openmod
            if (plugin is null)
            {
                _loadedTask = new TaskCompletionSource<object>();
                await Console.Out.WriteLineAsync("[Hydriuk.Plugins.Tools] - Plugin not activated. Waiting for plugin to load");
                await _loadedTask.Task;

                plugin = TryGetPlugin(pluginAssembly);
            }

            if (plugin is null)
                throw new Exception("Plugin could not be activated");

            if (plugin is not IAdaptablePlugin adaptablePlugin)
                throw new Exception("Plugin does not implement IAdaptablePlugin");

            return adaptablePlugin.ServiceProvider.GetRequiredService<TService>();
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
