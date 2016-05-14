using System.Collections.Generic;
using System.Windows.Input;
using tweetz5.Utilities.Translate;

namespace tweetz5.Controls
{
    public partial class ShortcutHelp
    {
        public ShortcutHelp()
        {
            InitializeComponent();
        }

        public struct KeyboardShortcut
        {
            public string Shortcut { get; set; }
            public string Description { get; set; }
        }

        private readonly KeyboardShortcut[] _keyboardShortcuts = 
        {
            new KeyboardShortcut {Shortcut = "Ctrl+X", Description = TranslationService.Instance.Translate("shortcut_help_close") as string},
            new KeyboardShortcut {Shortcut = "J", Description = TranslationService.Instance.Translate("shortcut_help_next") as string},
            new KeyboardShortcut {Shortcut = "K", Description = TranslationService.Instance.Translate("shortcut_help_previous") as string},
            new KeyboardShortcut {Shortcut = "R", Description = TranslationService.Instance.Translate("shortcut_help_reply") as string},
            new KeyboardShortcut {Shortcut = "T", Description = TranslationService.Instance.Translate("shortcut_help_retweet") as string},
            new KeyboardShortcut {Shortcut = "F", Description = TranslationService.Instance.Translate("shortcut_help_favorite") as string},
            new KeyboardShortcut {Shortcut = "N", Description = TranslationService.Instance.Translate("shortcut_help_new_status") as string},
            new KeyboardShortcut {Shortcut = "O", Description = TranslationService.Instance.Translate("shortcut_help_open_tweet") as string},
            new KeyboardShortcut {Shortcut = "L", Description = TranslationService.Instance.Translate("shortcut_help_open_link") as string},
            new KeyboardShortcut {Shortcut = "/", Description = TranslationService.Instance.Translate("shortcut_help_search") as string},
            new KeyboardShortcut {Shortcut = "Ctrl+Home", Description = TranslationService.Instance.Translate("shortcut_help_go_top") as string},
            new KeyboardShortcut {Shortcut = "Ctrl+End", Description = TranslationService.Instance.Translate("shortcut_help_go_bottom") as string},
            new KeyboardShortcut {Shortcut = "Ctrl+Return", Description = TranslationService.Instance.Translate("shortcut_help_send_status") as string}
        };

        public IEnumerable<KeyboardShortcut> KeyboardShortcuts => _keyboardShortcuts;

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}