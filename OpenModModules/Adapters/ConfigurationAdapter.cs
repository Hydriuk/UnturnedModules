using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol;
using System;

namespace Hydriuk.OpenModModules.Adapters
{
    public class ConfigurationAdapter<TConfiguration> : IConfigurationAdapter<TConfiguration> where TConfiguration : class, new()
    {
        public TConfiguration Configuration { get; private set; }

        public ConfigurationAdapter(IConfiguration configurator)
        {

            Configuration = new TConfiguration();

            //configurator.Bind(Configuration);
        }
    }
}
