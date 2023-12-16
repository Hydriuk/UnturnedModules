using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Hydriuk.OpenModModules.Adapters
{
    public class TranslationAdapter : ITranslationAdapter
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
                    LogError(arguments, ex, Assembly.GetCallingAssembly());
                    return this[key];
                }
            }
        }

        private readonly IStringLocalizer _translations;
        private readonly ILogger _logger;

        public TranslationAdapter(IStringLocalizer translations, ILogger logger)
        {
            _translations = translations;
            _logger = logger;
        }

        private void LogError(object arguments, FormatException ex, Assembly assembly)
        {
            PropertyInfo[] timeProperties = arguments.GetType().GetProperties();
            string propertiesString = timeProperties
                .Select(property => property.Name)
                .Aggregate((acc, curr) => $"{acc} {curr}");

            _logger.LogError(ex.Message);
            _logger.LogError($"Please, review your {assembly.FullName} translations file. One or more of the parameters is wrongly written.");
            _logger.LogError($"Available parameters are : {propertiesString}");
        }
    }
}