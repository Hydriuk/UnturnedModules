using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using System;
using System.Linq;
using System.Reflection;

namespace Hydriuk.RocketModModules.Adapters
{
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

            IAdaptablePlugin plugin = _serviceAdapter.GetAdaptablePlugin(pluginAssembly);

            return new Translations(plugin.Translations.Instance);
        }

        private class Translations : ITranslations
        {
            private readonly TranslationList _translations;

            public string this[string key]
            {
                get
                {
                    if (key == null)
                        throw new ArgumentNullException(nameof(key));

                    return _translations[key] ?? key;
                }
            }

            public string this[string key, object arguments]
            {
                get
                {
                    if (key == null)
                        throw new ArgumentNullException(nameof(key));

                    string[] splittedText = _translations[key].Split('{', '}');

                    PropertyInfo[] properties = arguments.GetType().GetProperties();
                    string[] values = new string[properties.Length];

                    for (int i = 1; i < splittedText.Length; i += 2)
                    {
                        PropertyInfo info = properties.FirstOrDefault(field => field.Name == splittedText[i]);

                        if (info == null)
                            continue;

                        var index = (i - 1) / 2;

                        values[index] = info.GetValue(arguments).ToString();

                        splittedText[i] = $"{{{index}}}";
                    }

                    string format = string.Join(string.Empty, splittedText);

                    return string.Format(format ?? key, values);
                }
            }

            public Translations(TranslationList translations)
            {
                _translations = translations;
            }
        }
    }
}