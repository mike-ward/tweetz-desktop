using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace tweetz5.Controls
{
    public partial class ShortcutHelp
    {
        private IInputElement _element;

        public ShortcutHelp()
        {
            InitializeComponent();
            Loaded += (sender, args) => Child.IsVisibleChanged += (o, eventArgs) =>
            {
                if (Child.IsVisible)
                {
                    _element = Keyboard.FocusedElement;
                    Keyboard.Focus(Child);
                }
            };
            Closed += (sender, args) => Keyboard.Focus(_element);
        }

        public struct KeyboardShortcut
        {
            public string Shortcut { get; set; }
            public string Description { get; set; }
        }

        private readonly KeyboardShortcut[] _keyboardShortcuts = new []
        {
            new KeyboardShortcut {Shortcut = "Ctrl + X", Description = "Close"},
            new KeyboardShortcut {Shortcut = "J", Description = "Next tweet"},
            new KeyboardShortcut {Shortcut = "K", Description = "Previous tweet"},
            new KeyboardShortcut {Shortcut = "R", Description = "Reply"},
            new KeyboardShortcut {Shortcut = "T", Description = "Retweet"},
            new KeyboardShortcut {Shortcut = "F", Description = "Favorite"},
            new KeyboardShortcut {Shortcut = "N", Description = "New tweet"},
            new KeyboardShortcut {Shortcut = "/", Description = "Search"},
            new KeyboardShortcut {Shortcut = "Ctrl + Home", Description = "Go to top"},
            new KeyboardShortcut {Shortcut = "Ctrl + End", Description = "Go to bottom"},
        };

        public IEnumerable<KeyboardShortcut> KeyboardShortcuts
        {
            get { return _keyboardShortcuts; }
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}