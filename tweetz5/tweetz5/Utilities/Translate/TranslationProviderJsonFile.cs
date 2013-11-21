using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace tweetz5.Utilities.Translate
{
    public class TranslationProviderJsonFile : ITranslationProvider
    {
        private Language _language;
        private CultureInfo _cultureInfo;
        private readonly Language[] _languages;

        public TranslationProviderJsonFile()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                var path = location + ".locale";
                var json = File.ReadAllText(path, Encoding.UTF8);
                var serializer = new JavaScriptSerializer();
                _languages = serializer.Deserialize<Language[]>(json);
            }
            catch (FileNotFoundException ex)
            {
                global::System.Console.WriteLine(ex.ToString());
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                return (_languages != null)
                    ? _languages.Select(t => new CultureInfo(t.Name))
                    : Enumerable.Empty<CultureInfo>();
            }
        }

        public object Translate(string key)
        {
            if (Thread.CurrentThread.CurrentUICulture.Equals(_cultureInfo) == false)
            {
                _cultureInfo = Thread.CurrentThread.CurrentUICulture;
                _language = FindLanguage(_cultureInfo);
            }
            string value;
            if (_language.Dictionary.TryGetValue(key, out value) == false)
            {
                _fallback.TryGetValue(key, out value);
            }
            return value;
        }

        private Language FindLanguage(CultureInfo culture)
        {
            var current = (_languages == null ? new Language() : null)
                ?? _languages.FirstOrDefault(t => t.Name == culture.Name)
                ?? _languages.FirstOrDefault(t => t.Name == culture.TwoLetterISOLanguageName)
                ?? new Language();
            return current;
        }

        private readonly Dictionary<string, string> _fallback = new Dictionary<string, string>
        {
            {"settings_show_media", "Show inline media"},
            {"settings_spell_check", "Spell check"}
        };

        public class Language
        {
            public string Name { get; set; }
            public Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        }
    }
}