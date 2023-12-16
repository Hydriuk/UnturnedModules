using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.Core.Plugins;
using System.Reflection;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ConfigurationAdapter<TConfiguration> : IConfigurationAdapter<TConfiguration> where TConfiguration : class, new()
    {
        public TConfiguration Configuration { get; private set; }

        public ConfigurationAdapter(TConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}