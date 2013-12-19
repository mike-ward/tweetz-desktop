using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using tweetz5.Utilities.System;

namespace tweetz5.Utilities
{
    internal class Win7FontConverter : IValueConverter
    {
        // Map Segoe UI Font to FontAwesome Font
        private static readonly Dictionary<string, string> FontDictionary = new Dictionary<string, string>
        {
            {"\xE179", "\xF0CA"},
            {"\xE10A", "\xF00D"},
            {"\xE10F", "\xF015"},
            {"\xE168", "\xF007"},
            {"\xE135", "\xF0E0"},
            {"\xE113", "\xF005"},
            {"\xE11A", "\xF002"},
            {"\xE115", "\xF013"},
            {"\xE104", "\xF040"},
            {"\xE107", "\xF014"},
            {"\xE248", "\xF112"},
            {"\xE1CA", "\xF079"},
            {"\xE082", "\xF005"},
            {"\xE10C", "\xF142"},
            {"\xE114", "\xF030"},
            {"\xE167", "\xF0C1"}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (BuildInfo.IsWindows8OrNewer) return parameter;
            string newValue;
            return (FontDictionary.TryGetValue((string) parameter, out newValue)) ? newValue : parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }
    }
}