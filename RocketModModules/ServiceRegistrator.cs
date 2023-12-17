using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hydriuk.RocketModModules
{
    public class ServiceRegistrator : IDisposable
    {
        protected readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private readonly IRocketPlugin _plugin;
        private readonly Assembly _pluginAssembly;

        public ServiceRegistrator(IRocketPlugin plugin)
        {
            _plugin = plugin;
            _pluginAssembly = _plugin.GetType().Assembly;
            _services = new Dictionary<Type, object>()
            {
                { _plugin.GetType(), _plugin }
            };

            if (R.Plugins.GetPlugin(_pluginAssembly) == null)
                R.Plugins.OnPluginsLoaded += ConfigureServices;
            else
                RocketPlugin.OnPluginLoading += ConfigureServices;
        }

        public void Dispose()
        {
            RocketPlugin.OnPluginLoading -= ConfigureServices;
            R.Plugins.OnPluginsLoaded -= ConfigureServices;

            foreach (object service in _services.Values)
            {
                if (service is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        private void ConfigureServices(IRocketPlugin plugin, ref bool cancelLoading)
        {
            if (plugin != _plugin)
                return;

            ConfigureServices();
        }

        private void ConfigureServices()
        {
            // Get services
            Dictionary<Type, PropertyInfo> serviceProperties = _plugin.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(PluginServiceAttribute)))
                .ToDictionary(p => p.PropertyType);

            // Init
            HashSet<ConstructorInfo> orderedConstructors = new HashSet<ConstructorInfo>();
            IEnumerable<Type> serviceTypes = serviceProperties
                .Select(p => p.Key);

            // Add services in dependency order
            AddParameters(orderedConstructors, serviceTypes);

            // Create services
            foreach (ConstructorInfo constructor in orderedConstructors)
            {
                if (constructor.DeclaringType == _plugin.GetType())
                    continue;

                List<object> parameters = new List<object>();
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType == typeof(RocketPlugin) || parameter.ParameterType == typeof(IRocketPlugin))
                    {
                        parameters.Add(_plugin);
                        continue;
                    }

                    parameters.Add(_services.Values.FirstOrDefault(s => s.GetType().GetInterfaces().Contains(parameter.ParameterType)));
                }

                object service = constructor.Invoke(parameters.ToArray());

                _services.Add(service.GetType(), service);

                if (serviceProperties.TryGetValue(service.GetType(), out PropertyInfo property))
                {
                    property.SetValue(_plugin, service);
                }
            }
        }

        private void AddParameters(HashSet<ConstructorInfo> orderedConstructors, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (_services.ContainsKey(type))
                    continue;

                // Try to find an implementing type from the plugin's assembly
                Type implementingType = _pluginAssembly
                    .GetTypes()
                    .Where(t => t.IsClass)
                    .Where(type.IsAssignableFrom)
                    .FirstOrDefault();

                // Try to find an implementing type from the RocketModModules' assembly
                if (implementingType == null)
                {
                    implementingType = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.IsClass)
                        .Where(type.IsAssignableFrom)
                        .FirstOrDefault();
                }

                if (implementingType == null)
                    throw new Exception($"Could not find implementing type for {type.Name}");

                // Get constructor of implementing type
                ConstructorInfo constructor = implementingType
                    .GetConstructors()
                    .FirstOrDefault();

                if (constructor == null)
                    throw new Exception($"Could not find any constructor for type {implementingType.Name}");

                if (orderedConstructors.Contains(constructor))
                    continue;

                // Get constructor parameters
                var paramTypes = constructor
                    .GetParameters()
                    .Select(p => p.ParameterType);

                AddParameters(orderedConstructors, paramTypes);

                orderedConstructors.Add(constructor);
            }
        }
    }
}