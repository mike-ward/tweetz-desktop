using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            _popup = CreatePopup(ea);
        }

        private static Popup CreatePopup(ExecutedRoutedEventArgs ea)
        {
            var popup = new Popup
            {
                AllowsTransparency = true,
                Child = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = new Image
                    {
                        Source = new BitmapImage(new Uri((string)ea.Parameter)),
                        Stretch = Stretch.UniformToFill
                    }
                },
                Placement = PlacementMode.Center,
                PlacementRectangle = new Rect(ScreenRect()),
                PopupAnimation = PopupAnimation.Fade
            };

            popup.KeyDown += (o, args) => popup.IsOpen = false;
            popup.MouseDown += (o, args) => popup.IsOpen = false;
            popup.IsOpen = true;
            return popup;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static Size ScreenRect()
        {
            var mainWindowPresentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            var m = mainWindowPresentationSource.CompositionTarget.TransformToDevice;
            var dpiWidthFactor = m.M11;
            var dpiHeightFactor = m.M22;
            var screenWidth = SystemParameters.PrimaryScreenWidth * dpiWidthFactor;
            var screenHeight = SystemParameters.PrimaryScreenHeight * dpiHeightFactor;
            return new Size(screenWidth, screenHeight);
        }
    }
}