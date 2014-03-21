using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace tweetz5.Utilities.Translate
{
    public class TranslationService
    {
        public readonly static TranslationService Instance = new TranslationService();
        public ITranslationProvider TranslationProvider { get; set; }
        public event EventHandler LanguageChanged;

        private TranslationService()
        {
        }

        public CultureInfo CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (value.Equals(Thread.CurrentThread.CurrentUICulture) == false)
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnLanguageChanged();
                }
            }
        }

        private void OnLanguageChanged()
        {
            if (LanguageChanged != null)
            {
                var handler = LanguageChanged;
                handler(this, EventArgs.Empty);
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get { return (TranslationProvider != null) ? TranslationProvider.Languages : Enumerable.Empty<CultureInfo>(); }
        }

        public object Translate(string key)
        {
            if (TranslationProvider != null)
            {
                var translatedValue = TranslationProvider.Translate(key);
                if (translatedValue != null)
                {
                    return translatedValue;
                }
            }
            return string.Format("!{0}!", key);
        }
    }
}