using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
using System;
using System.Linq;
using System.Reflection;

namespace Hydriuk.OpenModModules.Adapters
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class TranslationAdapter : ITranslationAdapter
    {
        private readonly IStringLocalizer _translations;

        public TranslationAdapter(IStringLocalizer translations)
        {
            _translations = translations;
        }

        public string this[string key] => _translations[key];

        public string this[string key, object arguments]
        {
            get
            {
                try
                {
                    return _translations[key, arguments];
                }
                catch (FormatException ex)
                {
                    PropertyInfo[] timeProperties = arguments.GetType().GetProperties();
                    string propertiesString = timeProperties
                        .Select(property => property.Name)
                        .Aggregate((acc, curr) => $"{acc} {curr}");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"[{DateTime.Now.ToString("T")}][BaseGuard] - {ex.Message}");
                    Console.WriteLine($"[BaseGuard] - Please, review your BaseGuard translations file.");
                    Console.WriteLine($"[BaseGuard] - One or more of the parameters is wrongly written.");
                    Console.WriteLine($"[BaseGuard] - Available parameters are : {propertiesString}");
                    Console.ResetColor();
                    return string.Empty;
                }
            }
        }
    }
}