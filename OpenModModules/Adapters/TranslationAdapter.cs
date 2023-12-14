using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class TranslationAdapter : ITranslationAdapter
    {
        private readonly IUnsafeServiceAdapter _serviceAdapter;

        public TranslationAdapter(IUnsafeServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public ITranslations GetTranslations()
        {
            Assembly pluginAssembly = Assembly.GetExecutingAssembly();

            IStringLocalizer translations = _serviceAdapter.GetService<IStringLocalizer>();
            ILogger logger = _serviceAdapter.GetService<ILogger>();

            return new Translations(translations, logger, pluginAssembly);
        }

        private class Translations : ITranslations
        {
            public string this[string key] => _translations?[key];

            public string this[string key, object arguments]
            {
                get
                {
                    try
                    {
                        return _translations?[key, arguments];
                    }
                    catch (FormatException ex)
                    {
                        LogError(arguments, ex);
                        return this[key];
                    }
                }
            }

            private readonly IStringLocalizer _translations;
            private readonly ILogger _logger;

            private readonly Assembly _pluginAssembly;

            public Translations(IStringLocalizer translations, ILogger logger, Assembly pluginAssembly)
            {
                _translations = translations;
                _logger = logger;

                _pluginAssembly = pluginAssembly;
            }

            private void LogError(object arguments, FormatException ex)
            {
                PropertyInfo[] timeProperties = arguments.GetType().GetProperties();
                string propertiesString = timeProperties
                    .Select(property => property.Name)
                    .Aggregate((acc, curr) => $"{acc} {curr}");

                _logger.LogError(ex.Message);
                _logger.LogError($"Please, review your {_pluginAssembly.FullName} translations file. One or more of the parameters is wrongly written.");
                _logger.LogError($"Available parameters are : {propertiesString}");
            }
        }
    }
}