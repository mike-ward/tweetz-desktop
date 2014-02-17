using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweetz5.Utilities.Translate;

namespace tweetz5UnitTests.Utilities.Translate
{
    [TestClass]
    public class ParseNameValueTextTests
    {
        [TestMethod]
        public void ParseNameShouldReturnEnglish()
        {
            const string text = "Name: English";
            var languages = TranslationProviderNameValueFile.Parse(text);
            languages[0].Name.Should().Be("English");
        }

        [TestMethod]
        public void ParseSampleTextShouldReturnTwoEntries()
        {
            const string text = "Name: English\r\nauthenticate_get_pin: Get PIN\r\nauthenticate_signin: Sign In\r\n";
            var languages = TranslationProviderNameValueFile.Parse(text);
            languages[0].Dictionary.Count.Should().Be(2);
            languages[0].Dictionary["authenticate_get_pin"].Should().Be("Get PIN");
            languages[0].Dictionary["authenticate_signin"].Should().Be("Sign In");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BadIndentifierFormatRaisesException()
        {
            const string text = "Name: English\r\nauthenticate_get_pin!: Get PIN\r\nauthenticate_signin: Sign In\r\n";
            var languages = TranslationProviderNameValueFile.Parse(text);
            languages.Should().BeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void EmptyValueFormatRaisesException()
        {
            const string text = "Name: English\r\nauthenticate_get_pin: \r\nauthenticate_signin: Sign In\r\n";
            var languages = TranslationProviderNameValueFile.Parse(text);
            languages.Should().BeNull();
        }

        [TestMethod]
        public void TheBigKahuna()
        {
            const string text = @"
Name:                           your two letter language code here then modify strings below
authenticate_instructions:      To get started click the Get PIN button. This opens a Web page where you'll authorize access. Copy the PIN from the Web page to here and click Sign In
authenticate_get_pin:           Get PIN
authenticate_signin:            Sign In
authenticate_about_pins:        PINs can only be used once so there's no need to save them.
                                  
profile_title:                  Profile Summary
profile_tweets:                 tweets
profile_friends:                friends
profile_followers:              followers
profile_follow:                 Follow
profile_following:              Following
profile_unfollow:               Unfollow
profile_follows_you:            follows you
                                  
compose_title_tweet:            Compose a tweet
compose_title_general_error:    Error
compose_title_shorten_error:    Error shortening urls
compose_send_button_tweet:      Tweet
compose_send_button_message:    Send
compose_tooltip_photo:          Include Photo (Drag and Drop supported)
compose_tooltip_shorten_links:  Shorten links 
                                  
status_reply:                   Reply
status_retweet:                 Retweet
status_favorite:                Favorite     
status_delete:                  Delete     
status_retweet_classic:         RT (Classic)     
status_copy:                    Copy     
status_copy_link:               Copy Link     

shortcut_help_close:            Close
shortcut_help_next:             Next Tweet
shortcut_help_previous:         Previous Tweet
shortcut_help_reply:            Reply
shortcut_help_retweet:          Retweet
shortcut_help_favorite:         Favorite
shortcut_help_new_status:       New Tweet
shortcut_help_search:           Search
shortcut_help_go_top:           Go to Top
shortcut_help_go_bottom:        Go to Bottom

time_ago_seconds:               {0}s
time_ago_minutes:               {0}m
time_ago_hours:                 {0}h
time_ago_days:                  {0}d
time_ago_date:                  MMM d
                                  
tooltip_unified_button:         Unified timeline (home mentions messages)
tooltip_home_button:            Home timeline
tooltip_mentions_button:        Mentions timeline
tooltip_messages_button:        Messages timeline
tooltip_favorites_button:       Favorites timeline
tooltip_search_button:          Search
tooltip_settings_button:        Settings
tooltip_compose_button:         Compose
                                  
settings_title:                 Settings
settings_chirp_on_update:       Chirp on timeline updates
settings_show_media:            Show inline media
settings_spell_check:           Spell check
settings_font_size:             Font Size
settings_sign_out:              Sign Out
settings_keyboard_help:         Press ? for keyboard shortcuts
settings_run_on_windows_start:  Run on windows start
settings_theme:                 Theme
settings_theme_dark:            Dark
settings_theme_light:           Light

==========================================

Name: en
authenticate_instructions:      To get started click the \Get PIN\ button. This opens a Web page where you'll authorize access. Copy the PIN from the Web page to here and click \Sign In\
authenticate_get_pin:           Get PIN
authenticate_signin:            Sign In
authenticate_about_pins:        PINs can only be used once so there's no need to save them.
                                  
profile_title:                  Profile Summary
profile_tweets:                 tweets
profile_friends:                friends
profile_followers:              followers
profile_follow:                 Follow
profile_following:              Following
profile_unfollow:               Unfollow
profile_follows_you:            follows you
                                  
compose_title_tweet:            Compose a tweet
compose_title_general_error:    Error
compose_title_shorten_error:    Error shortening urls
compose_send_button_tweet:      Tweet
compose_send_button_message:    Send
compose_tooltip_photo:          Include Photo (Drag and Drop supported)
compose_tooltip_shorten_links:  Shorten links 
                                  
status_reply:                   Reply
status_retweet:                 Retweet
status_favorite:                Favorite     
status_delete:                  Delete     
status_retweet_classic:         RT (Classic)     
status_copy:                    Copy     
status_copy_link:               Copy Link     

shortcut_help_close:            Close
shortcut_help_next:             Next Tweet
shortcut_help_previous:         Previous Tweet
shortcut_help_reply:            Reply
shortcut_help_retweet:          Retweet
shortcut_help_favorite:         Favorite
shortcut_help_new_status:       New Tweet
shortcut_help_search:           Search
shortcut_help_go_top:           Go to Top
shortcut_help_go_bottom:        Go to Bottom

time_ago_seconds:               {0}s
time_ago_minutes:               {0}m
time_ago_hours:                 {0}h
time_ago_days:                  {0}d
time_ago_date:                  MMM d
                                  
tooltip_unified_button:         Unified timeline (home mentions messages)
tooltip_home_button:            Home timeline
tooltip_mentions_button:        Mentions timeline
tooltip_messages_button:        Messages timeline
tooltip_favorites_button:       Favorites timeline
tooltip_search_button:          Search
tooltip_settings_button:        Settings
tooltip_compose_button:         Compose
                                  
settings_title:                 Settings
settings_chirp_on_update:       Chirp on timeline updates
settings_show_media:            Show inline media
settings_spell_check:           Spell check
settings_font_size:             Font Size
settings_sign_out:              Sign Out
settings_keyboard_help:         Press ? for keyboard shortcuts
settings_run_on_windows_start:  Run on windows start
settings_theme:                 Theme
settings_theme_dark:            Dark
settings_theme_light:           Light

==========================================

Name: de
authenticate_instructions:      Zum starten klick den [PIN holen] Button. Es öffnet sich die Webseite zur Authorisierung. Kopiere die PIN von der Seite ins Eingabefeld und klicke auf [Anmelden].
authenticate_get_pin:           PIN holen
authenticate_signin:            Anmelden
authenticate_about_pins:        PINs können nur einmal benutzt werden also kein Grund ihn zu speichern.
                                    
profile_title:                  Zusammenfassung
profile_tweets:                 tweets
profile_friends:                freunde
profile_followers:              follower
profile_follow:                 Folgen
profile_following:              Folge ich
profile_unfollow:               Entfolgen
profile_follows_you:            folgt dir
                                    
compose_title_tweet:            Verfasse einen tweet
compose_title_general_error:    Fehler
compose_title_shorten_error:    Fehler beim Link kürzen
compose_send_button_tweet:      Senden
compose_send_button_message:    Senden
compose_tooltip_photo:          Foto einfügen (Drag&Drop verfügbar)
compose_tooltip_shorten_links:  Links kürzen 
                                    
status_reply:                   Antworten
status_retweet:                 Retweeten
status_favorite:                Favorisieren     
status_delete:                  Löschen     
status_retweet_classic:         Retweeten (klassisch)     
status_copy:                    Kopieren     
status_copy_link:               Link kopieren     

shortcut_help_close:            Schliessen
shortcut_help_next:             nächster Tweet
shortcut_help_previous:         vorheriger Tweet
shortcut_help_reply:            Antworten
shortcut_help_retweet:          Weiterleiten
shortcut_help_favorite:         Favorisieren
shortcut_help_new_status:       Neuer Tweet
shortcut_help_search:           Suche
shortcut_help_go_top:           zum Anfang
shortcut_help_go_bottom:        zum Ende

time_ago_seconds:               {0}s
time_ago_minutes:               {0}m
time_ago_hours:                 {0}h
time_ago_days:                  {0}d
time_ago_date:                  MMM d
                                    
tooltip_unified_button:         Zusammenfassung (Timeline Erwähnungen Nachrichten)
tooltip_home_button:            Timeline
tooltip_mentions_button:        Erwähnungen
tooltip_messages_button:        Nachrichten
tooltip_favorites_button:       Favoriten
tooltip_search_button:          Suchen
tooltip_settings_button:        Einstellungen
tooltip_compose_button:         Verfassen
                                    
settings_title:                 Einstellungen
settings_chirp_on_update:       Zwitschern bei TL Neuigkeiten
settings_show_media:            Zeige Medien im tweet
settings_spell_check:           Rechtschreibprüfung
settings_font_size:             Schriftgröße
settings_sign_out:              Abmelden
settings_keyboard_help:         \nDrücke [?] zum Anzeigen der\nTastaturkurzbefehle
settings_run_on_windows_start:  Mit Windows starten
settings_theme:                 Thema
settings_theme_dark:            Dunkel
settings_theme_light:           Licht";

            var languages = TranslationProviderNameValueFile.Parse(text);
            languages.Length.Should().Be(3);
            languages[0].Name.Should().Be("your two letter language code here then modify strings below");
            languages[0].Dictionary["authenticate_get_pin"].Should().Be("Get PIN");
            languages[0].Dictionary["authenticate_signin"].Should().Be("Sign In");

            languages[1].Name.Should().Be("en");
            languages[1].Dictionary["authenticate_get_pin"].Should().Be("Get PIN");
            languages[1].Dictionary["authenticate_signin"].Should().Be("Sign In");

            languages[2].Name.Should().Be("de");
            languages[2].Dictionary["authenticate_get_pin"].Should().Be("PIN holen");
            languages[2].Dictionary["authenticate_signin"].Should().Be("Anmelden");
        }
    }
}