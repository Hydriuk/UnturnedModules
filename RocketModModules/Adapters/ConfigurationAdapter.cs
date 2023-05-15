using Hydriuk.UnturnedModules.API.Adapters;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
