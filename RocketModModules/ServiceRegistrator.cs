using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
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
        private readonly IServiceAdapter _serviceAdapter;

        public ServiceRegistrator(IRocketPlugin plugin)
        {
            _serviceAdapter = new ServiceAdapter();
            _plugin = plugin;

            Type pluginType = _plugin.GetType();
            _pluginAssembly = pluginType.Assembly;

            // Plugin type
            _services.Add(pluginType, _plugin);

            // Add plugin's base class
            _services.Add(pluginType.BaseType, _plugin);

            // Add interfaces implemented by the plugin
            foreach (var pluginInterface in _plugin.GetType().GetInterfaces())
                _services.Add(pluginInterface, _plugin);

            if (R.Plugins.GetPlugin(_pluginAssembly) == null)
                R.Plugins.OnPluginsLoaded += ConfigureServices;
            else
                RocketPlugin.OnPluginLoading += ConfigureServices;
        }

        public ServiceRegistrator(IRocketPlugin plugin, IRocketPluginConfiguration configuration): this(plugin)
        {
            // Add generic rocket plugin type
            _services.Add(plugin.GetType().BaseType.GetGenericTypeDefinition(), _plugin);

            // Add configuration type
            _services.Add(configuration.GetType(), configuration);

            // Add configuration implemented interfaces
            foreach (var configurationInterface in configuration.GetType().GetInterfaces())
                _services.Add(configurationInterface, configuration);
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

            _serviceAdapter.Dispose();
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

            // Get services instances from other plugins
            foreach (var serviceType in serviceTypes)
            {
                // Don't process if service is from the plugin or RocketModModules
                if (serviceType.Assembly == _pluginAssembly || serviceType.Assembly == Assembly.GetExecutingAssembly())
                    continue;

                object service = typeof(IServiceAdapter)
                    .GetMethod("GetService")
                    .MakeGenericMethod(new[] { serviceType })
                    .Invoke(_serviceAdapter, null);

                _services.Add(service.GetType(), service);
            }

            // Add services in dependency order
            AddParameters(orderedConstructors, serviceTypes);

            // Create services
            foreach (ConstructorInfo constructor in orderedConstructors)
            {
                if (_services.ContainsKey(constructor.DeclaringType))
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

                Type implementingType = type.Assembly
                        .GetTypes()
                        .Where(t => t.IsClass)
                        .Where(type.IsAssignableFrom)
                        .FirstOrDefault();

                if (implementingType == null)
                    throw new Exception($"Could not find implementing type for {type.Name}");

                // Get constructor of implementing type
                ConstructorInfo constructor = implementingType
                    .GetConstructors()
                    .FirstOrDefault();

                if (constructor == null)
                    throw new Exception($"Could not find any constructor for type {implementingType.Name}");

                // Stop if constructor is already added
                if (orderedConstructors.Contains(constructor))
                    continue;

                // Add parameters of the constructor
                var paramTypes = constructor
                    .GetParameters()
                    .Select(p => p.ParameterType);

                AddParameters(orderedConstructors, paramTypes);

                orderedConstructors.Add(constructor);
            }
        }
    }
}