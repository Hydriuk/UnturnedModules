using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

            // Add plugin types and all variants
            _services.Add(pluginType, _plugin);
            _services.Add(typeof(RocketPlugin), _plugin);
            if(pluginType.BaseType != typeof(RocketPlugin))
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

        private async void ConfigureServices()
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
                // Try to add the service if its from another plugin
                await TryAddExternService(serviceType);
            }

            // Add services in dependency order
            await AddParameters(orderedConstructors, serviceTypes);

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

        private async Task AddParameters(HashSet<ConstructorInfo> orderedConstructors, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (_services.ContainsKey(type))
                    continue;

                // Try to add the service if its from another plugin
                if (await TryAddExternService(type))
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

                await AddParameters(orderedConstructors, paramTypes);

                orderedConstructors.Add(constructor);
            }
        }

        private async Task<bool> TryAddExternService(Type serviceType)
        {
            if (_services.ContainsKey(serviceType))
                return false;

            // Don't process if service is from the plugin or RocketModModules
            if (serviceType.Assembly == _pluginAssembly || serviceType.Assembly == Assembly.GetExecutingAssembly())
                return false;

            // Call GetServiceAsync<T> with a dynamic T type
            object task = typeof(IServiceAdapter)
                .GetMethod("GetServiceAsync", new[] { typeof(Assembly) })
                .MakeGenericMethod(new[] { serviceType })
                .Invoke(_serviceAdapter, new[] { serviceType.Assembly });

            // Wait for the task to end
            object result = await GetTaskResultAsync(task, serviceType);

            // Add service and implementation types
            _services.Add(serviceType, result);
            if(serviceType != result.GetType())
                _services.Add(result.GetType(), result);

            return true;
        }

        public async Task<object> GetTaskResultAsync(object task, Type type)
        {
            // Get the Task<T> type dynamically
            Type taskType = typeof(Task<>).MakeGenericType(type);

            // Get the result property of the Task<T> using reflection
            PropertyInfo resultProperty = taskType.GetProperty("Result");

            // Check if the taskObject is of the correct type
            if (!taskType.IsAssignableFrom(task.GetType()))
                throw new ArgumentException("The provided taskObject is not of the expected Task<T> type.");

            // Await the taskObject dynamically
            await (Task)task;

            return resultProperty.GetValue(task);
        }
    }
}