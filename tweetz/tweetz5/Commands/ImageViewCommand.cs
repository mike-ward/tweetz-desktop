using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            mediaElement.MediaFailed += (s, e) => mediaElement.Source = new Uri(media.MediaUrl);

            var popup = new Popup
            {
                Child = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Child = mediaElement
                },
                Placement = PlacementMode.Center,
                PlacementRectangle = Screen.ScreenRectFromWindow(window),
                PopupAnimation = PopupAnimation.Fade,
                RenderTransform = new ScaleTransform(1.25, 1.25),
                SnapsToDevicePixels = true
            };

            popup.KeyDown += (o, args) => popup.IsOpen = false;
            popup.MouseDown += (o, args) => popup.IsOpen = false;
            popup.IsOpen = true;
            return popup;
        }

        private static MediaElement CreateMediaElement(Uri uri)
        {
            // All this to make a video loop

            var mediaTimeline = new MediaTimeline
            {
                Source = uri,
                RepeatBehavior = RepeatBehavior.Forever
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(mediaTimeline);

            var beginStoryboard = new BeginStoryboard {Storyboard = storyboard};

            var eventTrigger = new EventTrigger {RoutedEvent = FrameworkElement.LoadedEvent};
            eventTrigger.Actions.Add(beginStoryboard);

            var mediaElement = new MediaElement();
            mediaElement.Triggers.Add(eventTrigger);

            return mediaElement;
        }

        private static Uri MediaSource(Media media)
        {
            if (media.VideoInfo?.Variants?[0] == null)
            {
                return new Uri(media.MediaUrl);
            }

            var url = media.VideoInfo.Variants
                .Where(variant => IsMp4(variant.Url))
                .Select(variant => variant.Url)
                .FirstOrDefault();

            return url != null
                ? new Uri(url.Replace("https://", "http://"))
                : new Uri(media.MediaUrl);
        }

        private static bool IsMp4(string url)
        {
            var findExtension = new Regex(@".+(\.\w{3})\?*.*");
            var result = findExtension.Match(url);

            return result.Success && 
                   result.Groups.Count > 1 &&
                   string.Equals(result.Groups[1].Value, ".mp4", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}