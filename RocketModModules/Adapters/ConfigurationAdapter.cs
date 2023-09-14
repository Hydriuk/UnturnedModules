using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ConfigurationAdapter<T> : IConfigurationAdapter<T> where T : IRocketPluginConfiguration, new()
    {
        public T Configuration { get; }

        public ConfigurationAdapter(T configuration)
        {
            Configuration = configuration;
        }
    }
}