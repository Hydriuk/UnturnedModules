using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.Core.Plugins;
using System.Reflection;

namespace Hydriuk.RocketModModules.Adapters
{
    internal class ConfigurationAdapter : IConfigurationAdapter
    {
        private readonly IUnsafeServiceAdapter _serviceAdapter;

        public ConfigurationAdapter(IUnsafeServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public TConfiguration GetConfiguration<TConfiguration>() where TConfiguration : class, new()
        {
            Assembly pluginAssembly = Assembly.GetExecutingAssembly();

            IRocketPlugin<TConfiguration> plugin = _serviceAdapter.GetAdaptablePlugin(pluginAssembly) as IRocketPlugin<TConfiguration>;

            return plugin.Configuration.Instance;
        }
    }
}