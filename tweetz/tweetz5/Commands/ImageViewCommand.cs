using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using tweetz5.Model;
using tweetz5.Utilities.System;

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
            var media = (Media)ea.Parameter;
            var uri = MediaSource(media);
            var mediaElement = CreateMediaElement(uri);

            var popup = new Popup
            {
                AllowsTransparency = true,
                Child = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Child = mediaElement
                },
                Placement = PlacementMode.Center,
                PlacementRectangle = Screen.ScreenRectFromWindow(window),
                PopupAnimation = PopupAnimation.Fade,
            };

            popup.KeyDown += (o, args) => popup.IsOpen = false;
            popup.MouseDown += (o, args) => popup.IsOpen = false;
            popup.IsOpen = true;
            return popup;
        }

        private static MediaElement CreateMediaElement(Uri uri)
        {
            var mediaElement = new MediaElement
            {
                Source = uri,
                Stretch = Stretch.UniformToFill,
                LoadedBehavior = MediaState.Play
            };

            mediaElement.MediaFailed += (s, e) => MessageBox.Show(e.ErrorException.Message);
            return mediaElement;
        }

        private static Uri MediaSource(Media media)
        {

            if (media.VideoInfo?.Variants?[0] == null)
            {
                return new Uri(media.MediaUrl);
            }

            var url = media.VideoInfo.Variants
                .Where(variant => variant.Url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                .Select(variant => variant.Url)
                .FirstOrDefault() ?? "null.mp4";

            return new Uri(url.Replace("https://", "http://"));
        }
    }
}