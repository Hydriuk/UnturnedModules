using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
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
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class ConfigurationAdapter : IConfigurationAdapter
    {
        private readonly IUnsafeServiceAdapter _serviceAdapter;

        public ConfigurationAdapter(IUnsafeServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public TConfiguration GetConfiguration<TConfiguration>()
            where TConfiguration : class, new()
        {
            Assembly pluginAssembly = Assembly.GetExecutingAssembly();

            TConfiguration configuration = new TConfiguration();
            IConfiguration configurator = _serviceAdapter.GetService<IConfiguration>(pluginAssembly);

            configurator.Bind(configuration);

            return configuration;
        }
    }
}
