using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal abstract class ConfigurationAdapter : IConfigurationAdapter
    {
        private readonly IServiceAdapter _serviceAdapter;

        public ConfigurationAdapter(IServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public async Task<IConfProxy<TConfiguration>> GetConfiguration<TPlugin, TConfiguration>()
            where TPlugin : IAdaptablePlugin
            where TConfiguration : new()
        {
            TConfiguration configuration = new TConfiguration();
            IConfiguration configurator = await _serviceAdapter.GetServiceAsync<TPlugin, IConfiguration>();

            configurator.Bind(configuration);

            return new ConfProxy<TConfiguration>(configuration);
        }

        private class ConfProxy<T> : IConfProxy<T>
        {
            public T Configuration { get; }

            public ConfProxy(T configuration)
            {
                Configuration = configuration;
            }
        }
    }
}
