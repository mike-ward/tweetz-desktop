using System.Collections.Generic;

namespace tweetz5.Utilities.Translate
{
    internal static class Fallback
    {
        public static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            {
                "authenticate_instructions",    "To get started, click the \"Get PIN\" button. " +
                                                "This opens a Web page where you'll authorize access. " +
                                                "Copy the PIN from the Web page to here and click \"Sign In\""
            },
            {"authenticate_get_pin",            "Get PIN"},
            {"authenticate_signin",             "Sign In"},
            {"authenticate_about_pins",         "PINs can only be used once so there's no need to save them."},
            {"profile_title",                   "Profile Summary"},
            {"profile_tweets",                  "tweets"},
            {"profile_friends",                 "friends"},
            {"profile_followers",               "followers"},
            {"profile_follow",                  "Follow"},
            {"profile_following",               "Following"},
            {"profile_unfollow",                "Unfollow"},
            {"profile_follows_you",             "follows you"},
            {"compose_title_tweet",             "Compose a tweet"},
            {"compose_title_general_error",     "Error"},
            {"compose_title_shorten_error",     "Error shortening urls"},
            {"compose_send_button_tweet",       "Tweet"},
            {"compose_send_button_message",     "Send"},
            {"compose_tooltip_photo",           "Include Photo (Drag and Drop supported)"},
            {"compose_tooltip_shorten_links",   "Shorten links"},
            {"status_reply",                    "Reply"},
            {"status_retweet",                  "Retweet"},
            {"status_favorite",                 "Favorite"},
            {"status_delete",                   "Delete"},
            {"status_retweet_classic",          "RT (Classic)"},
            {"status_copy",                     "Copy"},
            {"status_copy_link",                "Copy Link"},
            {"shortcut_help_close",             "Close"},
            {"shortcut_help_next",              "Next Tweet"},
            {"shortcut_help_previous",          "Previous Tweet"},
            {"shortcut_help_reply",             "Reply"},
            {"shortcut_help_retweet",           "Retweet"},
            {"shortcut_help_favorite",          "Favorite"},
            {"shortcut_help_new_status",        "New Tweet"},
            {"shortcut_help_search",            "Search"},
            {"shortcut_help_go_top",            "Go to Top"},
            {"shortcut_help_go_bottom",         "Go to Bottom"},
            {"time_ago_seconds",                "{0}s"},
            {"time_ago_minutes",                "{0}m"},
            {"time_ago_hours",                  "{0}h"},
            {"time_ago_days",                   "{0}d"},
            {"time_ago_date",                   "MMM d"},
            {"tooltip_unified_button",          "Unified timeline (home, mentions, messages)"},
            {"tooltip_home_button",             "Home timeline"},
            {"tooltip_mentions_button",         "Mentions timeline"},
            {"tooltip_messages_button",         "Messages timeline"},
            {"tooltip_favorites_button",        "Favorites timeline"},
            {"tooltip_search_button",           "Search"},
            {"tooltip_settings_button",         "Settings"},
            {"tooltip_compose_button",          "Compose"},
            {"settings_title",                  "Settings"},
            {"settings_chirp_on_update",        "Chirp on timeline updates"},
            {"settings_show_media",             "Show inline media"},
            {"settings_spell_check",            "Spell check"},
            {"settings_font_size",              "Font Size"},
            {"settings_sign_out",               "Sign Out"},
            {"settings_keyboard_help",          "Press ? for keyboard shortcuts"},
            {"settings_run_on_windows_start",   "Run on windows start"},
            {"settings_theme",                  "Theme"},
            {"settings_theme_dark",             "Dark"},
            {"settings_theme_light",            "Light"}
        };
    }
}