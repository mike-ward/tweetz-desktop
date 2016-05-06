using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace tweetz5.Utilities.Translate
{
    public class TranslationProviderNameValueFile : ITranslationProvider
    {
        private Language _language;
        private CultureInfo _cultureInfo;
        private readonly Language[] _languages;

        public TranslationProviderNameValueFile()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                var path = location + ".locale";
                var text = File.ReadAllText(path, Encoding.UTF8);
                _languages = Parse(text);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                return _languages?.Select(t => new CultureInfo(t.TwoLetterLanguageCode)) ?? Enumerable.Empty<CultureInfo>();
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
                Fallback.English.TryGetValue(key, out value);
            }
            return value;
        }

        private Language FindLanguage(CultureInfo culture)
        {
            return _languages.FirstOrDefault(t => t.TwoLetterLanguageCode == culture.TwoLetterISOLanguageName) ?? new Language();
        }

        public static Language[] Parse(string text)
        {
            Language language = null;
            var languages = new List<Language>();
            var regex = new Regex(@"^[a-zA-Z][_a-zA-Z0-9]*$");

            foreach (var line in text.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("=")) continue;

                var pair = line.Split(new[] {':'}, 2);
                if (pair.Length != 2) throw new FormatException(ErrorMessage("invalid expression", line));

                var name = pair[0];
                var value = pair[1].Trim();

                if (regex.IsMatch(name) == false) throw new FormatException(ErrorMessage("invalid identifier", line));
                if (string.IsNullOrWhiteSpace(value)) throw new FormatException(ErrorMessage("empty value", line));

                if (name == "TwoLetterLanguageCode")
                {
                    language = new Language {TwoLetterLanguageCode = value};
                    languages.Add(language);
                }
                else
                {
                    language.Dictionary.Add(name, value);
                }
            }
            return languages.ToArray();
        }

        private static string ErrorMessage(string message, string line)
        {
            return $"{message}: {line}";
        }

        public class Language
        {
            public string TwoLetterLanguageCode { get; set; }
            public readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        }
    }
}