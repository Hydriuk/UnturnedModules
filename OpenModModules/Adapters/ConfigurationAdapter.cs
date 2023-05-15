using Hydriuk.UnturnedModules.API.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ConfigurationAdapter<T> : IConfigurationAdapter<T> where T : new()
    {
        public T Configuration { get; }

        public ConfigurationAdapter(IConfiguration configurator)
        {
            Configuration = new T();

            configurator.Bind(Configuration);
        }
    }
}
