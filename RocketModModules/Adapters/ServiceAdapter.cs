using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ServiceAdapter : IServiceAdapter
    {
        private readonly IAdaptablePlugin _plugin;

        public ServiceAdapter(IAdaptablePlugin plugin)
        {
            _plugin = plugin;
        }

        public Task<TService> GetServiceAsync<TService>()
        {
            PropertyInfo serviceInfo = _plugin
                .GetType()
                .GetProperties()
                .Where(property => property.PropertyType == typeof(TService))
                .FirstOrDefault();

            return Task.FromResult((TService)serviceInfo.GetValue(_plugin));
        }
    }
}
