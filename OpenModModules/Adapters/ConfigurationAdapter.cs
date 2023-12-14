using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    internal class ConfigurationAdapter<TConfiguration> : IConfigurationAdapter<TConfiguration> where TConfiguration : class, new()
    {
        public TConfiguration Configuration {  get; private set; }

        public ConfigurationAdapter(IConfiguration configurator)
        {
            Configuration = new TConfiguration();
            configurator.Bind(Configuration);
        }
    }
}
