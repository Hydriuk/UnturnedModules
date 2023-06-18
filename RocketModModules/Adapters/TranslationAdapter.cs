using Hydriuk.UnturnedModules.Adapters;
using Rocket.API.Collections;
using System;
using System.Linq;
using System.Reflection;

namespace Hydriuk.RocketModModules.Adapters
{
    public class TranslationAdapter : ITranslationAdapter
    {
        private readonly TranslationList _translations;

        public TranslationAdapter(TranslationList translations)
        {
            _translations = translations;
        }

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
    }
}