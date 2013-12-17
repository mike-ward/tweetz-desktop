using System;
using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    public class ChangeTheme
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                SetTheme((string)ea.Parameter);
            }
            catch (Exception)
            {
                SetTheme("Dark");
            }
        }

        private static void SetTheme(string theme)
        {
            Func<string, Uri> assetPath = n => new Uri(string.Format("/Assets/Themes/Classic/{0}.xaml", n), UriKind.Relative);
            var colorDictionary = (ResourceDictionary)Application.LoadComponent(assetPath(theme));
            var commonDictionary = (ResourceDictionary)Application.LoadComponent(assetPath("Common"));

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(colorDictionary);
            Application.Current.Resources.MergedDictionaries.Add(commonDictionary);
        }
    }
}