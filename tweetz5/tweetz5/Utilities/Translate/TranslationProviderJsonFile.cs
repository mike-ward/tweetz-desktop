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
            {"authenticate_instructions",      "To get started, click the \"Get PIN\" button. " +
                                               "This opens a Web page where you'll authorize access. " +
                                               "Copy the PIN from the Web page to here and click \"Sign In\""},
            {"authenticate_get_pin",           "Get PIN"},
            {"authenticate_signin",            "Sign In"},
            {"authenticate_about_pins",        "PINs can only be used once so there's no need to save them."},
                                               
            {"profile_title",                  "Profile Summary"},
            {"profile_tweets",                 "tweets"},
            {"profile_friends",                "friends"},
            {"profile_followers",              "followers"},
            {"profile_follow",                 "Follow"},
            {"profile_following",              "Following"},
            {"profile_unfollow",               "Unfollow"},
            {"profile_follows_you",            "follows you"},

            {"compose_title_tweet",            "Compose a tweet"},
            {"compose_title_general_error",    "Error"},
            {"compose_title_shorten_error",    "Error shortening urls"},
            {"compose_send_button_tweet",      "Tweet"},
            {"compose_send_button_message",    "Send"},
            {"compose_tooltip_photo",          "Include Photo (Drag and Drop supported)"},
            {"compose_tooltip_shorten_links",  "Shorten links"}, 

            {"tooltip_unified_button",         "Unified timeline (home, mentions, messages)"},
            {"tooltip_home_button",            "Home timeline"},
            {"tooltip_mentions_button",        "Mentions timeline"},
            {"tooltip_messages_button",        "Messages timeline"},
            {"tooltip_favorites_button",       "Favorites timeline"},
            {"tooltip_search_button",          "Search"},
            {"tooltip_settings_button",        "Settings"},
            {"tooltip_compose_button",         "Compose"},
                                               
            {"settings_title",                 "Settings"},
            {"settings_chirp_on_update",       "Chirp on timeline updates"},
            {"settings_show_media",            "Show inline media"},
            {"settings_spell_check",           "Spell check"},
            {"settings_font_size",             "Font Size"},
            {"settings_sign_out",              "Sign Out"},
            {"settings_keyboard_help",         "Press ? for keyboard shortcuts"}
        };

        public class Language
        {
            public string Name { get; set; }
            public Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        }
    }
}