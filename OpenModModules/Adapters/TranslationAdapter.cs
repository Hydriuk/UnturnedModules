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
        private readonly IServiceAdapter _serviceAdapter;

        public TranslationAdapter(IServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        public async Task<ITranslations> GetTranslations<T>() where T : IAdaptablePlugin
        {
            IStringLocalizer translations = await _serviceAdapter.GetServiceAsync<T, IStringLocalizer>();
            ILogger<T> logger = await _serviceAdapter.GetServiceAsync<T, ILogger<T>>();

            return new Translations<T>(translations, logger);
        }

        private class Translations<T> : ITranslations
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
            private readonly ILogger<T> _logger;

            public Translations(IStringLocalizer translations, ILogger<T> logger)
            {
                _translations = translations;
                _logger = logger;
            }

            private void LogError(object arguments, FormatException ex)
            {
                PropertyInfo[] timeProperties = arguments.GetType().GetProperties();
                string propertiesString = timeProperties
                    .Select(property => property.Name)
                    .Aggregate((acc, curr) => $"{acc} {curr}");

                _logger.LogError(ex.Message);
                _logger.LogError($"Please, review your {typeof(T).Assembly.FullName} translations file. One or more of the parameters is wrongly written.");
                _logger.LogError($"Available parameters are : {propertiesString}");
            }
        }
    }
}