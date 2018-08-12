using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Screen = tweetz5.Utilities.System.Screen;

namespace tweetz5.Commands
{
    internal static class ImageViewCommand
    {
        private static Popup _popup;
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            if (_popup != null) _popup.IsOpen = false;
            _popup = CreatePopup(sender as Window, ea);
        }

        private static Popup CreatePopup(Window window, ExecutedRoutedEventArgs ea)
        {
            var popup = new Popup
            {
                AllowsTransparency = true,
                Child = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Child = new MediaElement
                    {
                        Source = new Uri((string)ea.Parameter),
                        Stretch = Stretch.UniformToFill,
                        LoadedBehavior = MediaState.Play
                    }
                },
                Placement = PlacementMode.Center,
                PlacementRectangle = Screen.ScreenRectFromWindow(window),
                PopupAnimation = PopupAnimation.Fade
            };

            popup.KeyDown += (o, args) => popup.IsOpen = false;
            popup.MouseDown += (o, args) => popup.IsOpen = false;
            popup.IsOpen = true;
            return popup;
        }
    }
}